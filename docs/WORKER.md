# Worker et modération des avis

Le worker .NET prend en charge la modération en arrière-plan. L’API enregistre l’avis au statut `Pending` et répond `202 Accepted` avec un identifiant et un en-tête `Location` vers la route de suivi. Le worker publie ensuite l’événement de création dans RabbitMQ, consomme ce message et applique la décision dans PostgreSQL. Le navigateur consulte l’API pour connaître le nouveau statut. Si le worker ou RabbitMQ est temporairement indisponible, l’avis et le message à publier restent en base.

```mermaid
flowchart LR
    UI[Frontend Blazor] -->|Envoi et suivi de l’avis| API[API ASP.NET Core]
    API -->|Avis Pending et message dans la même transaction| DB[(PostgreSQL MyDisk)]
    subgraph W [Processus Worker]
        Pub[RabbitMqOutboxPublisher]
        Con[ReviewCreatedConsumer]
        Mod[ModerateReviewCommandHandler]
        Con --> Mod
    end
    DB -->|Lignes OutboxMessages à publier| Pub
    Pub -->|Publication review.created| MQ[Exchange mydisk.events]
    MQ --> Q[File mydisk.review.created]
    Q --> Con
    Mod -->|Lecture du texte et mise à jour du statut| DB
    Q -.->|Rejet définitif ou limite de relivraison| DLQ[File mydisk.dead-letter]
```

## Classes du circuit asynchrone

| Classe ou contrat | Rôle |
|---|---|
| [`Worker/Program.cs`](../apps/api/src/Worker/Program.cs) | Démarre le Generic Host, raccorde les services applicatifs et charge `ReviewModeration` et `RabbitMq`. |
| [`Messaging.DependencyInjection`](../apps/api/src/Messaging/DependencyInjection.cs) et [`RabbitMqOptions`](../apps/api/src/Messaging/RabbitMqOptions.cs) | Valident la configuration du broker et enregistrent le publieur et le consommateur comme services hébergés. Chaque service ouvre sa propre connexion et son propre canal. |
| [`CreateReviewCommandHandler`](../apps/api/src/Services/Reviews/CreateReviewCommandHandler.cs) et [`NewReviewCreatedDomainEvent`](../apps/api/src/Domain/Reviews/NewReviewCreatedDomainEvent.cs) | Retrouvent le disque du propriétaire, y attachent un nouvel avis et signalent sa création. |
| [`EntitySaveChangesInterceptor`](../apps/api/src/Data/EntitySaveChangesInterceptor.cs) et [`MediatRDomainEventDispatcher`](../apps/api/src/Services/MediatRDomainEventDispatcher.cs) | Extraient les événements des entités suivies par EF Core et les diffusent en mémoire avant l’écriture SQL. |
| [`NewReviewCreatedDomainEventHandler`](../apps/api/src/Services/Reviews/NewReviewCreatedDomainEventHandler.cs) et [`ReviewCreatedMessage`](../apps/api/src/Services/Reviews/ReviewCreatedMessage.cs) | Convertissent l’événement de domaine en un message `review.created` contenant `ReviewId`, `DiskId` et `PublishedDate`. Cette date est initialisée à la création de l’avis. |
| [`IOutboxWriter`](../apps/api/src/Services/Messaging/IOutboxWriter.cs) et [`OutboxWriter`](../apps/api/src/Data/Messaging/OutboxWriter.cs) | Sérialisent le message en JSON et l’ajoutent au `ApplicationDbContext` courant. L’identifiant de corrélation reprend le `TraceId` W3C courant quand il existe, ou un GUID généré. |
| [`OutboxEntry`](../apps/api/src/Data.Configurations/Outbox/OutboxEntry.cs) et [`OutboxConfiguration`](../apps/api/src/Data.Configurations/Outbox/OutboxConfiguration.cs) | Définissent la ligne `OutboxMessages`, le contenu `jsonb`, le suivi des tentatives et l’index des lignes à traiter. |
| [`IOutboxStore`](../apps/api/src/Services/Messaging/IOutboxStore.cs), [`OutboxMessage`](../apps/api/src/Services/Messaging/OutboxMessage.cs) et [`OutboxStore`](../apps/api/src/Data/Messaging/OutboxStore.cs) | Lisent et verrouillent un lot éligible, le passent au publieur via un callback et enregistrent le résultat en base. |
| [`RabbitMqOutboxPublisher`](../apps/api/src/Messaging/RabbitMqOutboxPublisher.cs) | Interroge l’outbox, publie les messages persistants et attend la confirmation du broker pour chaque envoi. |
| [`RabbitMqConsumer<TMessage>`](../apps/api/src/Messaging/RabbitMqConsumer.cs) | Gère l’abonnement, la désérialisation, la portée DI de chaque livraison et les acquittements manuels. |
| [`ReviewCreatedConsumer`](../apps/api/src/Messaging/ReviewCreatedConsumer.cs) et [`ModerateReviewCommandRequest`](../apps/api/src/Services/Reviews/ModerateReviewCommandRequest.cs) | Écoutent `mydisk.review.created` et transmettent à MediatR une commande portant l’identifiant de l’avis. |
| [`ModerateReviewCommandHandler`](../apps/api/src/Services/Reviews/ModerateReviewCommandHandler.cs) et [`ReviewRepository`](../apps/api/src/Data/ReviewRepository.cs) | Relisent l’avis en base, appliquent la décision uniquement si son statut est `Pending` et enregistrent le nouveau statut. |
| [`IReviewModerator`](../apps/api/src/Services/Reviews/Moderation/IReviewModerator.cs) et [`KeywordReviewModerator`](../apps/api/src/Services/Reviews/Moderation/KeywordReviewModerator.cs) | Découplent la décision du transport. L’implémentation normalise la casse, les accents et les séparateurs du titre et du contenu, puis cherche des expressions interdites définies dans le code. |
| [`ReviewModerationResult`](../apps/api/src/Services/Reviews/Moderation/ReviewModerationResult.cs) et [`ReviewModerationSettings`](../apps/api/src/Services/Reviews/Moderation/ReviewModerationSettings.cs) | Représentent le verdict et son motif éventuel, et permettent de simuler la latence de la décision. Le motif est consigné dans les logs du worker. |

## Création de l’avis et écriture de l’outbox

L’outbox transactionnelle désigne la table PostgreSQL qui conserve les messages à envoyer. L’avis et son message sont enregistrés dans la même transaction via le `ApplicationDbContext` partagé. Un échec de l’écriture annule les deux. L’API et le worker ont ainsi chacun leur propre étape, et l’API peut accepter un avis pendant une interruption du broker.

```mermaid
sequenceDiagram
    actor U as Propriétaire du disque
    participant API as ReviewController
    participant P as Pipeline MediatR et UnitOfWork
    participant H as CreateReviewCommandHandler
    participant EF as ApplicationDbContext et intercepteur
    participant E as NewReviewCreatedDomainEventHandler
    participant O as OutboxWriter
    participant DB as PostgreSQL

    U->>API: POST /api/review avec JWT et données de l’avis
    API->>P: Envoyer CreateReviewCommandRequest avec le propriétaire
    P->>P: Valider la commande
    P->>DB: Ouvrir la transaction
    P->>H: Exécuter la commande
    H->>DB: Charger le disque par ID et propriétaire iss + sub
    alt Disque accessible
        H->>EF: Ajouter le Review Pending au disque et enregistrer l’événement sur le Review
        H-->>P: Identifiant de l’avis
        P->>EF: SaveChangesAsync
        EF->>E: Diffuser NewReviewCreatedDomainEvent via MediatR
        E->>O: Enqueue de review.created
        O->>EF: Ajouter OutboxEntry au même contexte
        EF->>DB: Insérer l’avis et la ligne OutboxMessages
        P->>DB: Valider la transaction
        P-->>API: Identifiant de l’avis
        API-->>U: 202 Accepted, statut Pending et Location
    else Disque absent ou inaccessible
        H-->>P: ObjectNotFoundException
        P->>DB: Annuler la transaction
        API-->>U: 404 Not Found
    end
```

## Publication dans RabbitMQ

Le publieur traite jusqu’à 50 lignes par lot. Il attend 2 secondes si le lot a publié moins de 50 messages. La sélection porte sur les lignes dont `ProcessedOnUtc` est vide et dont `NextRetryOnUtc` est vide ou arrivé à échéance. `FOR UPDATE SKIP LOCKED` permet à plusieurs publieurs de traiter des lots distincts en ignorant les lignes verrouillées. Chaque message utilise sa clé de routage `review.created`, porte son identifiant d’outbox comme `MessageId` et dispose d’un délai de publication de 15 secondes. [Sélection SQL](../apps/api/src/Data/Messaging/OutboxStore.cs) · [Publication RabbitMQ](../apps/api/src/Messaging/RabbitMqOutboxPublisher.cs)

```mermaid
sequenceDiagram
    participant P as RabbitMqOutboxPublisher
    participant O as OutboxStore
    participant DB as PostgreSQL
    participant MQ as RabbitMQ

    P->>MQ: Ouvrir un canal avec publisher confirms
    loop Tant que le canal est ouvert
        P->>O: PublishPendingAsync avec une limite de 50
        O->>DB: Sélectionner et verrouiller les lignes éligibles
        loop Jusqu’à la fin du lot ou au premier échec
            O->>P: Appeler le callback de publication
            P->>MQ: Publier le JSON persistant vers mydisk.events, mandatory
            alt Publication confirmée
                MQ-->>P: Confirmation du broker
                P-->>O: Publication terminée
                O->>O: Renseigner ProcessedOnUtc
            else Échec ou délai dépassé
                P-->>O: Exception de publication
                O->>O: Enregistrer l’échec et arrêter le lot
            end
        end
        opt Lot contenant des lignes
            O->>DB: Enregistrer les résultats et valider la transaction
        end
        O-->>P: Nombre de publications confirmées
        opt Moins de 50 publications
            P->>P: Attendre 2 secondes
        end
    end
```

La première erreur d’un lot interrompt son traitement. Les lignes précédentes confirmées sont marquées comme traitées, la ligne en échec reçoit une nouvelle échéance, et les lignes suivantes restent éligibles. L’intervalle entre tentatives commence à 5 secondes puis double jusqu’à 5 minutes. `RetryCount` et `LastError` renseignent l’échec de publication. Le code conserve les lignes traitées et continue de réessayer les échecs de publication sans seuil terminal. [État de l’outbox](../apps/api/src/Data.Configurations/Outbox/OutboxEntry.cs)

```mermaid
stateDiagram-v2
    state "Éligible à la publication" as Eligible
    state "En attente de la prochaine tentative" as Retry
    state "Publication enregistrée en base" as Processed
    [*] --> Eligible : insertion de la ligne
    Eligible --> Processed : confirmation RabbitMQ et commit PostgreSQL
    Eligible --> Retry : échec enregistré dans le lot
    Retry --> Eligible : échéance NextRetryOnUtc atteinte
```

## Consommation, décision et relivraison

L’exchange `mydisk.events` route `review.created` vers la file quorum durable `mydisk.review.created`. Le consommateur limite le nombre de livraisons non acquittées à 10 avec le prefetch. Il confirme chaque livraison par `ACK` après la fin de la commande MediatR, qui inclut la validation de la transaction. La topologie est importée au démarrage du `StatefulSet` RabbitMQ. [Définitions RabbitMQ](../apps/messaging/k8s/base/definitions.json) · [Démarrage du broker](../apps/messaging/k8s/base/statefulset.yml)

```mermaid
sequenceDiagram
    participant Q as File mydisk.review.created
    participant C as ReviewCreatedConsumer
    participant H as ModerateReviewCommandHandler
    participant M as KeywordReviewModerator
    participant DB as PostgreSQL
    participant DLQ as File mydisk.dead-letter

    Q->>C: Livrer review.created
    C->>C: Désérialiser le JSON
    alt Message illisible ou nul
        C->>Q: REJECT avec requeue désactivé
        Q->>DLQ: Router via l’exchange mydisk.dead-letter
    else Message désérialisé
        C->>H: Envoyer ModerateReviewCommandRequest via MediatR
        H->>DB: Charger l’avis par ReviewId
        alt Avis absent ou statut différent de Pending
            H-->>C: Succès sans modification après le retour du pipeline
            C->>Q: ACK
        else Avis Pending et traitement réussi
            H->>M: Évaluer le titre et le contenu
            alt Contenu approuvé
                M-->>H: Verdict approuvé
                H->>DB: Enregistrer Published
            else Expression interdite détectée
                M-->>H: Verdict rejeté
                H->>DB: Enregistrer Rejected
            end
            H-->>C: Succès après validation de la transaction MediatR
            C->>Q: ACK
        else Erreur de traitement
            H-->>C: Exception après l’annulation de la transaction
            C->>C: Journaliser et attendre 1 seconde
            C->>Q: NACK avec requeue activé
            opt Limite de livraison atteinte
                Q->>DLQ: Router via l’exchange mydisk.dead-letter
            end
        end
    end
```

| Situation | Effet observable |
|---|---|
| Connexion ou canal RabbitMQ interrompu | Le publieur et le consommateur retentent la connexion après 5 secondes. |
| Confirmation RabbitMQ reçue avant un arrêt ou un échec de validation de l’outbox | Le message peut être republié si `ProcessedOnUtc` n’a pas été enregistré. Ce champ indique la publication, indépendamment du résultat de modération. |
| Livraison répétée après une modération déjà enregistrée | Le handler constate le statut différent de `Pending` et le consommateur acquitte la livraison sans modifier l’avis. Ce contrôle couvre les rediffusions successives. |
| Archivage pendant une modération | Le worker vérifie le statut lors de la lecture. Si l’archivage est validé ensuite, une écriture concurrente du statut par le worker peut l’écraser. Le mapping EF Core du statut ne définit aucun jeton de concurrence. |
| Avis supprimé avec son disque avant le traitement | Le handler constate son absence, termine normalement et le consommateur acquitte la livraison. |
| JSON illisible ou message nul | Le consommateur rejette la livraison et la politique RabbitMQ la dirige vers `mydisk.dead-letter`. |
| Échecs répétés de modération | La file quorum applique une politique `delivery-limit` de `5`. Au terme des relivraisons autorisées, RabbitMQ déplace le message vers `mydisk.dead-letter`. L’avis peut rester `Pending` et aucun consommateur de cette file n’est enregistré. |

[Lecture et écriture du verdict](../apps/api/src/Services/Reviews/ModerateReviewCommandHandler.cs) · [Dépôt des avis](../apps/api/src/Data/ReviewRepository.cs) · [Mapping EF Core](../apps/api/src/Data.Configurations/Reviews/ReviewConfiguration.cs)

## États d’un avis

Le statut `Rejected` est une décision de modération réussie. Les erreurs techniques de traitement suivent le circuit de relivraison RabbitMQ. L’archivage relève d’une action explicite du propriétaire du disque ou d’un administrateur. [Agrégat `Review`](../apps/api/src/Domain/Reviews/Review.cs) · [Archivage propriétaire](../apps/api/src/Services/Reviews/ArchiveReviewCommandHandler.cs) · [Archivage administrateur](../apps/api/src/Services/Reviews/ArchiveReviewAsAdminCommand.cs)

```mermaid
stateDiagram-v2
    [*] --> Pending : avis créé
    Pending --> Published : contenu approuvé par le worker
    Pending --> Rejected : contenu rejeté par le worker
    Pending --> Archived : archivage
    Published --> Archived : archivage
    Rejected --> Archived : archivage
```

## Suivi de l’avis dans le frontend

`CollectionWorkspace` démarre le suivi à l’envoi d’un avis ou à l’ouverture d’un disque contenant un avis `Pending`. `ReviewModerationWatcher` appelle `GET /api/review/{reviewId}`. L’API limite la lecture au propriétaire du disque. L’intervalle démarre à 1 seconde, double jusqu’à 8 secondes et varie légèrement à chaque essai. Le temps de suivi visé est de 60 secondes. L’interface signale un statut final ou invite à consulter le disque plus tard si l’attente dépasse ce délai. [Orchestration Blazor](../apps/frontend/src/Components/CollectionWorkspace.razor.cs) · [Scrutation du statut](../apps/frontend/src/Services/ReviewModerationWatcher.cs) · [Lecture côté API](../apps/api/src/Services/Reviews/GetReviewQueryHandler.cs)

```mermaid
sequenceDiagram
    participant UI as CollectionWorkspace
    participant W as ReviewModerationWatcher
    participant API as API MyDisk
    participant DB as PostgreSQL

    UI->>W: Suivre un avis Pending par son ID
    loop Jusqu’à un statut final, un 404, l’annulation ou l’expiration
        W->>W: Attendre avec délai progressif et variation aléatoire
        W->>API: GET /api/review/{reviewId} avec JWT
        API->>DB: Chercher l’avis du propriétaire
        alt Avis Pending ou erreur API transitoire
            API-->>W: 200 Pending ou erreur transitoire
            W->>W: Vérifier le délai de suivi
        else Avis Published, Rejected ou Archived
            API-->>W: 200 avec le nouveau statut
            W-->>UI: Décision disponible
            UI->>UI: Actualiser le détail et signaler le statut
        else Avis introuvable
            API-->>W: 404 Not Found
            W-->>UI: Arrêter le suivi de cet avis
        end
    end
    opt Délai dépassé
        W-->>UI: TimeoutException
        UI->>UI: Signaler que le statut sera actualisé au prochain chargement
    end
```

## Exécution et diagnostic en local

Sur k3d, `make dev-up` déploie RabbitMQ et un réplica du worker dans la pile locale. Le worker démarre après le job de migrations et utilise le même compte PostgreSQL applicatif que l’API. L’overlay local injecte les identifiants RabbitMQ via `rabbitmq-credentials`, configure `RabbitMq__HostName=mydisk-messaging` et ajoute 3 secondes de latence simulée avec `ReviewModeration__SimulatedLatency`. La configuration `RabbitMq` demande un hôte, un utilisateur et un mot de passe. Les autres valeurs par défaut sont le port `5672`, le vhost `/` et l’exchange `mydisk.events`. [Déploiement worker](../apps/api/k8s/base/worker-deployment.yml) · [Overlay local](../apps/api/k8s/overlays/local/kustomization.yml) · [Makefile](../Makefile)

```sh
make k8s-status
make k8s-logs-worker
```

Pour suivre un avis, partir de l’identifiant renvoyé dans le `202` et chercher `ReviewId` dans les logs de modération. Si la publication échoue, les logs du publieur indiquent le `MessageId`, qui correspond à l’identifiant de la ligne d’outbox. Examiner `NextRetryOnUtc`, `RetryCount` et `LastError` permet de situer l’échec avant publication.

L’overlay AKS actuel retire le `Deployment` du worker et omet RabbitMQ. L’API peut toujours créer l’avis et sa ligne d’outbox. Les avis restent `Pending` jusqu’à une action d’archivage ou au traitement par un worker connecté à cet environnement. [Overlay API cloud](../apps/api/k8s/overlays/cloud/kustomization.yml) · [Composition cloud](../deploy/k8s/cloud/kustomization.yml)
