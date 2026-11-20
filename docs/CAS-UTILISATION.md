# Cas d’utilisation de MyDisk

MyDisk permet à chaque compte de gérer sa collection de disques et ses avis. Le rôle `admin` donne accès à un écran d’administration des avis de toutes les collections. Ce guide relie les actions de l’interface aux routes HTTP et aux traitements qui les exécutent. [Architecture](ARCHITECTURE.md) · [Responsabilités des classes](CLASSES.md) · [Worker et modération](WORKER.md)

## Acteurs et cas d’utilisation

| Acteur | Actions |
| --- | --- |
| Visiteur | Se connecter via Keycloak. Les sondes HTTP sont aussi accessibles sans session. |
| Membre | Gérer sa collection, soumettre et archiver ses avis, consulter leur statut, vérifier sa session et consulter l’état de l’API. |
| Administrateur MyDisk | Utiliser les fonctions de membre et consulter ou archiver les avis de toutes les collections avec le rôle client `admin`. |
| Exploitant ou client de supervision | Interroger les sondes de santé et l’indicateur de maintenance. Un développeur peut consulter le contrat HTTP dans Swagger. |
| Keycloak | Authentifier le compte, émettre les jetons et terminer la session à la déconnexion. |

```mermaid
flowchart LR
    V["Visiteur"]
    M["Membre connecté"]
    A["Administrateur MyDisk"]
    O["Exploitant ou supervision"]
    K["Keycloak"]

    subgraph MyDisk
        S(["Se connecter ou se déconnecter"])
        C(["Consulter, filtrer et trier sa collection"])
        D(["Créer, modifier ou supprimer un disque"])
        R(["Associer un artiste existant"])
        N(["Soumettre un avis et suivre sa modération"])
        P(["Consulter et archiver ses avis"])
        Q(["Consulter et archiver les avis de toutes les collections"])
        U(["Consulter son profil et vérifier sa session"])
        H(["Vérifier la santé et la maintenance de l’API"])
        T(["Lancer la démonstration de retry"])
    end

    V --- S
    M --- S
    M --- C
    M --- D
    M --- R
    M --- N
    M --- P
    M --- U
    M --- H
    M --- T
    A --- Q
    O --- H
    S --- K
```

L’API identifie le propriétaire à partir du couple `iss` / `sub` du jeton validé. Les opérations personnelles filtrent sur ce propriétaire, y compris quand le compte possède le rôle `admin`. Une ressource absente de la collection du compte est traitée comme introuvable. Les anciens disques dont le propriétaire n’est pas renseigné restent hors des collections personnelles. [Identité du propriétaire](../apps/api/src/Api/Controllers/DiskOwnerClaims.cs) · [Tests d’isolation](../apps/api/src/Api.IntegrationTests/Controllers/PrivateCollectionFixture.cs)

## Routes HTTP

| Action | Méthode et route | Accès | Réponse nominale |
| --- | --- | --- | --- |
| Vérifier le jeton présenté à l’API | `GET /api/auth/me` | Jeton valide | `200` et claims |
| Lister les disques et leurs avis publiés | `GET /api/Disk/disks` | Propriétaire | `200` et liste, éventuellement vide |
| Consulter un disque par son nom exact | `GET /api/Disk/disk-by-name?name=…` | Propriétaire | `200` et détail du disque |
| Ajouter un disque | `POST /api/Disk/create-disk` | Session avec `iss` et `sub` | `204` |
| Modifier un disque | `PUT /api/Disk/update-disk` | Propriétaire | `204` |
| Supprimer un disque | `DELETE /api/Disk/delete-disk?diskId=…` | Propriétaire | `204` |
| Associer un artiste | `POST /api/Disk/attach-author` | Propriétaire | `204` |
| Soumettre un avis | `POST /api/Review` | Propriétaire du disque | `202` avec l’identifiant, `Pending` et un en-tête `Location` |
| Consulter un avis par identifiant | `GET /api/Review/{reviewId}` | Propriétaire du disque | `200` et détail de l’avis |
| Archiver un avis depuis le détail du disque | `POST /api/Review/Archive` | Propriétaire du disque | `204` |
| Lister les avis de toutes les collections | `GET /api/admin/reviews` | Rôle `admin` | `200` et liste tous statuts confondus |
| Archiver un avis depuis l’administration | `POST /api/admin/reviews/{reviewId}/archive` | Rôle `admin` | `204` |
| Tester le processus, la disponibilité ou la santé globale | `GET /health/live`, `GET /health/ready`, `GET /health` | Public | `200` et `Healthy` si la sonde réussit |
| Lire l’état de maintenance | `GET /maintenance` | Public | `200` avec `Active` ou `Inactive` |
| Déclencher la démonstration de retry | `GET /api/Miscellaneous/disks` | Jeton valide | `204` si l’exercice se termine |
| Explorer le contrat HTTP | `GET /swagger/index.html` | Public | `200` et interface Swagger UI |

Les routes protégées répondent `401` si l’authentification échoue. Une session valide sans les claims requis pour l’opération reçoit `403`. Les validations FluentValidation et les erreurs de format traitées par le filtre MVC renvoient `400`. Le filtre convertit les ressources introuvables en `404`. En mode maintenance, le filtre bloque les actions MVC concernées avec `503`. [Contrôleurs](../apps/api/src/Api/Controllers/) · [Validation JWT et politique Admin](../apps/api/src/Api/KeycloakAuthentication.cs) · [Filtres MVC](../apps/api/src/Api/Filters/)

## Connexion, profil et déconnexion

Toutes les pages fonctionnelles du frontend passent par `RequireSession`. Le client public Keycloak utilise le flux OIDC Authorization Code avec PKCE S256 dans la configuration locale. Les appels protégés suivants emploient un jeton d’accès dont l’API valide la signature, l’expiration, l’émetteur et l’audience. Le profil affiche les claims de la session Blazor et offre une vérification distincte des claims du jeton reçu par l’API. [Routage Blazor](../apps/frontend/src/App.razor) · [Realm local](../apps/keycloak/k8s/overlays/local/mydisk-realm.json) · [Profil](../apps/frontend/src/Pages/ProfilePage.razor)

```mermaid
sequenceDiagram
    actor U as Utilisateur
    participant B as Frontend Blazor
    participant K as Keycloak
    participant A as API

    U->>B: Ouvre Ma collection
    B->>B: RequireSession vérifie la session
    opt Session absente
        B->>K: Redirection OIDC avec challenge PKCE
        K-->>U: Formulaire de connexion
        U->>K: Authentification
        K-->>B: Redirection vers login-callback avec le code
        B->>K: Échange du code avec le vérificateur PKCE
        K-->>B: Jetons de la session
    end
    B-->>U: Ouvre la page protégée
    U->>B: Dans Mon profil, vérifie la connexion à l’API
    B->>A: GET /api/auth/me avec jeton Bearer
    A->>A: Valide le jeton
    alt Jeton accepté
        A-->>B: 200 avec les claims validés
        B-->>U: Affiche les claims
    else Jeton refusé
        A-->>B: 401
        B-->>U: Affiche l’erreur de session
    end
    opt Déconnexion
        U->>B: Demande la déconnexion
        B->>K: Termine la session OIDC
        K-->>B: Redirection vers logout-callback
        B-->>U: Revient au parcours de connexion
    end
```

## Liste de collection et détail d’un disque

La page **Ma collection** filtre côté navigateur par nom de disque ou pseudonyme d’artiste et trie par nom ou date de sortie. La page **Avis** part de la même liste privée et affiche ses avis publiés. Le détail d’un disque peut également afficher les avis en attente et rejetés. Les avis archivés sont écartés du résultat disque. La recherche du détail se fait par nom exact. Si plusieurs disques partagent un nom et que l’identifiant renvoyé diffère de la sélection, l’interface affiche un message d’ambiguïté. [Navigation et chargement](../apps/frontend/src/Components/CollectionWorkspace.razor.cs) · [Dépôt EF Core](../apps/api/src/Data/DiskRepository.cs) · [Projection du disque](../apps/api/src/Services/Disks/MapperProfiles.cs)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as CollectionWorkspace
    participant A as DiskController
    participant H as Handlers de lecture
    participant P as PostgreSQL

    U->>B: Ouvre Ma collection ou Avis
    B->>A: GET /api/Disk/disks
    A->>H: GetAllDisksQueryRequest avec le propriétaire
    H->>P: Lit ses disques, artistes et avis publiés
    P-->>H: Résultats de la collection
    H-->>A: Liste de DiskResult
    A-->>B: 200 avec la collection
    B-->>U: Affiche les disques ou les avis publiés
    U->>B: Ouvre le détail d’un disque
    B->>A: GET /api/Disk/disk-by-name avec le nom exact
    A->>H: GetDiskByNameQueryRequest avec le propriétaire
    H->>P: Cherche le disque et charge ses avis
    alt Disque trouvé
        P-->>H: Disque et avis
        H-->>A: DiskResult avec avis non archivés
        A-->>B: 200 avec le détail
        alt Identifiant identique à la sélection
            B-->>U: Affiche le détail
        else Identifiant différent
            B-->>U: Signale un nom ambigu
        end
    else Disque introuvable
        A-->>B: 404
        B-->>U: Affiche l’erreur
    end
```

## Création, modification et suppression d’un disque

Le formulaire Blazor demande un nom de 30 caractères au maximum et une date de sortie. À la création, le serveur prend le propriétaire dans le jeton et crée un `NewDiskCreatedDomainEvent`, dont le gestionnaire journalise l’identifiant pendant la sauvegarde EF Core. La mise à jour API applique les champs fournis. La suppression nécessite une confirmation dans l’interface et entraîne la suppression en cascade des avis du disque. [Éditeur Blazor](../apps/frontend/src/Components/DiskEditor.razor.cs) · [Commandes disques](../apps/api/src/Services/Disks/) · [Relation disque et avis](../apps/api/src/Data.Configurations/Reviews/ReviewConfiguration.cs)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as CollectionWorkspace
    participant A as DiskController
    participant C as Pipeline et handler MediatR
    participant P as PostgreSQL

    U->>B: Valide le formulaire ou confirme la suppression
    alt Ajouter
        B->>A: POST /api/Disk/create-disk
        A->>C: CreateDiskCommandRequest avec iss et sub
        C->>C: Construit le disque et ajoute son événement de création
        C->>P: Enregistre le disque
    else Modifier
        B->>A: PUT /api/Disk/update-disk
        A->>C: UpdateDiskCommandRequest avec le propriétaire
        C->>P: Charge le disque par ID et propriétaire
        C->>P: Enregistre les champs fournis
    else Supprimer
        B->>A: DELETE /api/Disk/delete-disk
        A->>C: DeleteDiskCommandRequest avec le propriétaire
        C->>P: Charge le disque par ID et propriétaire
        C->>P: Supprime le disque et ses avis
    end
    alt Opération réussie
        C-->>A: Écriture terminée
        A-->>B: 204
        B->>A: Recharge la collection et le détail encore ouvert
        B-->>U: Affiche la confirmation
    else Disque absent de la collection pour modifier ou supprimer
        C-->>A: ObjectNotFoundException
        A-->>B: 404
        B-->>U: Affiche l’erreur
    end
```

## Association d’un artiste existant

Dans le détail, l’utilisateur sélectionne un artiste déjà présent dans les disques chargés ou saisit son UUID. Le serveur charge le disque du compte et l’artiste correspondant. `Disk.AttachAuthor` exige que la date de naissance de l’artiste précède la date de sortie lorsqu’elle est renseignée. [Formulaire d’association](../apps/frontend/src/Components/DiskDetail.razor.cs) · [Handler](../apps/api/src/Services/Disks/AttachAuthorCommandHandler.cs) · [Règle de domaine](../apps/api/src/Domain/Disks/Disk.cs)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as CollectionWorkspace
    participant A as DiskController
    participant H as AttachAuthorCommandHandler
    participant P as PostgreSQL

    U->>B: Sélectionne un artiste ou saisit son UUID
    B->>A: POST /api/Disk/attach-author
    A->>H: AttachAuthorCommandRequest avec le propriétaire
    H->>P: Cherche le disque par ID et propriétaire, puis l’artiste par ID
    alt Disque et artiste trouvés, dates compatibles
        H->>H: Disk.AttachAuthor
        H->>P: Enregistre l’association
        A-->>B: 204
        B->>A: Recharge la collection et le détail
    else Une des ressources est introuvable
        A-->>B: 404
        B-->>U: Affiche l’erreur
    else Dates incompatibles
        H-->>A: InvalidOperationException
        A-->>B: 500
        B-->>U: Affiche l’erreur
    end
```

## Soumission et suivi d’un avis

Le formulaire recueille un titre de 30 caractères au maximum, un texte et une note de 0 à 5. L’interface masque le formulaire jusqu’à la date de sortie du disque. Cette règle de date s’applique dans la fiche Blazor. La création enregistre l’avis `Pending` et son message d’outbox dans la même transaction, puis renvoie `202`. Le frontend interroge le statut par identifiant avec un délai progressif et aléatoire, selon une échéance de 60 secondes vérifiée entre les tentatives. Il s’arrête lorsque l’avis quitte `Pending` ou que l’API répond `404`. Rouvrir le détail d’un disque relance le suivi des avis toujours en attente. [Fiche Blazor](../apps/frontend/src/Components/DiskDetail.razor) · [Contrôleur](../apps/api/src/Api/Controllers/ReviewController.Post.cs) · [Suivi dans Blazor](../apps/frontend/src/Services/ReviewModerationWatcher.cs) · [Publication et consommation du message](WORKER.md)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as Frontend Blazor
    participant A as ReviewController
    participant M as Services et EF Core
    participant P as PostgreSQL
    participant W as Worker de modération

    U->>B: Soumet le formulaire d’avis
    B->>A: POST /api/Review
    A->>M: CreateReviewCommandRequest avec le propriétaire
    M->>M: Valide la commande
    opt Commande valide
        M->>P: Cherche le disque par ID et propriétaire
    end
    alt Commande valide et disque trouvé
        M->>M: Ajoute l’avis Pending et déclenche son événement de création
        M->>P: Enregistre l’avis et l’entrée d’outbox dans la même transaction
        A-->>B: 202 avec ID, Pending et Location
        B-->>U: Indique que l’avis est en cours de modération
        Note over P,W: Le worker traite le message de façon asynchrone
        loop Jusqu’à décision, 404 ou expiration du suivi
            B->>A: GET /api/Review/{reviewId}
            A->>M: GetReviewQueryRequest avec le propriétaire
            M->>P: Lit l’avis par ID et propriétaire du disque
            P-->>M: Statut courant ou avis introuvable
            A-->>B: 200 avec le statut ou 404
        end
        opt Décision reçue ou délai dépassé
            B-->>U: Actualise le statut ou signale l’attente prolongée
        end
    else Disque introuvable ou commande invalide
        A-->>B: 404 ou 400
        B-->>U: Affiche l’erreur
    end
```

## Archivage d’un avis par son propriétaire

L’action de la fiche disque passe par la même vérification de propriété que la lecture directe. Elle change le statut à `Archived`. Un avis archivé disparaît de la fiche et de la liste des avis publiés au rechargement. Il reste consultable par son propriétaire via la route de lecture directe et reste visible dans la liste d’administration. [Handler d’archivage](../apps/api/src/Services/Reviews/ArchiveReviewCommandHandler.cs) · [Handler de lecture](../apps/api/src/Services/Reviews/GetReviewQueryHandler.cs) · [Projection du disque](../apps/api/src/Services/Disks/MapperProfiles.cs)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as Détail du disque
    participant A as ReviewController
    participant H as ArchiveReviewCommandHandler
    participant P as PostgreSQL

    U->>B: Archive son avis
    B->>A: POST /api/Review/Archive avec reviewId
    A->>H: ArchiveReviewCommandRequest avec le propriétaire
    H->>P: Cherche l’avis par ID et propriétaire du disque
    alt Avis trouvé
        H->>H: Review.Archive
        H->>P: Persiste le statut Archived
        A-->>B: 204
        B->>A: Recharge la collection et le détail
        B-->>U: Confirme l’archivage
    else Avis introuvable
        A-->>B: 404
        B-->>U: Affiche l’erreur
    end
```

## Administration des avis

Le frontend affiche l’entrée **Administration → Avis** aux comptes pour lesquels `mydisk_roles` contient `admin`. Le contrôleur applique sa propre politique `Admin` sur le JWT et exige aussi `sub`. La liste inclut tous les statuts et le nom du disque. Lors d’un premier archivage, le contrôleur journalise l’émetteur, le sujet et l’identifiant de l’avis après l’écriture. Répéter l’opération sur un avis déjà archivé renvoie également `204`, sans nouvelle écriture de l’avis. [Écran d’administration](../apps/frontend/src/Components/AdminReviewsView.razor) · [Contrôleur](../apps/api/src/Api/Controllers/AdminReviewsController.cs) · [Handler](../apps/api/src/Services/Reviews/ArchiveReviewAsAdminCommand.cs)

```mermaid
sequenceDiagram
    actor U as Administrateur
    participant B as AdminReviewsView
    participant A as API avec politique Admin
    participant H as Handlers d’administration
    participant P as PostgreSQL

    U->>B: Ouvre Administration puis Avis
    B->>A: GET /api/admin/reviews
    alt Politique Admin satisfaite
        A->>H: GetAdminReviewsQuery
        H->>P: Lit les avis de toutes les collections avec leurs disques
        P-->>H: Avis tous statuts confondus
        A-->>B: 200 avec la liste
        U->>B: Confirme le retrait d’un avis
        B->>A: POST /api/admin/reviews/{reviewId}/archive
        A->>A: Applique de nouveau la politique Admin
        A->>H: ArchiveReviewAsAdminCommand
        H->>P: Cherche l’avis par ID
        alt Avis à archiver
            H->>P: Passe l’avis à Archived
            A->>A: Journalise l’archivage
            A-->>B: 204
            B-->>U: Affiche le statut Archivé
        else Avis déjà archivé
            A-->>B: 204
        else Avis introuvable
            A-->>B: 404
        end
    else Jeton absent, invalide ou politique refusée
        A-->>B: 401 ou 403
    end
```

## Sondes, maintenance et démonstration de retry

L’écran **État de l’API** appelle les trois sondes ainsi que `/maintenance` en parallèle. `/health/live` vérifie que le processus HTTP répond. `/health/ready` exécute la vérification PostgreSQL lorsque la connexion est configurée et `/health` agrège les vérifications enregistrées. Le mode `MaintenanceSettings:Global` bloque les actions MVC soumises au filtre. Les sondes, `/maintenance` et la route minimale `/api/auth/me` restent accessibles selon leurs règles d’accès habituelles. [Sondes et routes](../apps/api/src/Api/Program.cs) · [Enregistrement du contrôle PostgreSQL](../apps/api/src/Api/DependencyInjection.cs) · [Écran État de l’API](../apps/frontend/src/Components/Diagnostics.razor.cs)

```mermaid
sequenceDiagram
    actor U as Membre
    participant B as Diagnostics
    participant A as API
    participant P as PostgreSQL
    participant R as RetryServiceHandler et Polly

    U->>B: Ouvre État de l’API
    par Liveness
        B->>A: GET /health/live
        A-->>B: Résultat du contrôle du processus
    and Readiness et santé globale
        B->>A: GET /health/ready et GET /health
        opt Vérification PostgreSQL configurée
            A->>P: Vérifie la connectivité
            P-->>A: Résultat du contrôle
        end
        A-->>B: Résultats des sondes
    and Maintenance
        B->>A: GET /maintenance
        A-->>B: 200 avec Active ou Inactive
    end
    B-->>U: Affiche les résultats
    opt Lancement de la démonstration de retry
        U->>B: Lance le test de retry
        B->>A: GET /api/Miscellaneous/disks avec jeton Bearer
        A->>R: RetryServiceRequest
        R->>R: Applique les politiques sur un résultat puis sur une exception
        A-->>B: 204 si le traitement se termine
        B-->>U: Affiche le résultat du test
    end
```

La démonstration exécute deux scénarios aléatoires via `PollyRetryService`, avec un nombre de reprises et un délai configurables via `RetryServiceSettings`. Les politiques de reprise du flux de modération sont documentées avec le worker. [Scénarios de démonstration](../apps/api/src/Services/Miscellaneous/RetryServiceHandler.cs) · [Implémentation Polly](../apps/api/src/RetryService/PollyRetryService.cs) · [Reprises du worker](WORKER.md)
