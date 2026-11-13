# Créer MyDisk avec GitHub Actions

## Première configuration

La création est déclenchée dans GitHub, avec le workflow **Infrastructure AKS**, action **apply**. Elle prépare le stockage d’état, AKS puis la configuration Kubernetes.

L’application Azure `mydisk-github` utilise une identité fédérée pour le dépôt `Fabian-Raeskinet/mydisk-api` et l’environnement GitHub `production`. Le subject OIDC doit correspondre à celui émis par GitHub pour ce dépôt.

Sur la souscription choisie, attribuer à cette application les deux rôles suivants une fois.

- **Contributeur** pour créer les groupes de ressources, le stockage et AKS.
- **Contributeur aux données Blob du stockage** pour lire et écrire les états avec Entra ID. Le périmètre souscription permet l’accès au compte dès sa création. Il autorise aussi l’accès aux autres données Blob de cette souscription. Une souscription dédiée à l’apprentissage permet de limiter ce périmètre.

Le workflow utilise ces droits existants. Le rôle de gestion des attributions IAM reste à l’administrateur qui effectue cette configuration initiale.

Dans GitHub, ouvrir `Settings → Environments → production`. Limiter les branches autorisées à `master`.

| Variable d’environnement GitHub | Valeur |
|---|---|
| `AZURE_CLIENT_ID` | ID d’application de `mydisk-github` |
| `AZURE_TENANT_ID` | ID du tenant Entra |
| `AZURE_SUBSCRIPTION_ID` | Souscription choisie |
| `TF_STATE_STORAGE_ACCOUNT` | Nom unique, 3-24 lettres minuscules ou chiffres, par exemple `mydisktfstatefabian` |
| `AKS_RESOURCE_GROUP` | Facultatif, défaut `rg-mydisk` |
| `AKS_CLUSTER_NAME` | Facultatif, défaut `mydisk` |
| `AZURE_LOCATION` | Facultatif, défaut `westeurope` |
| `AKS_NODE_VM_SIZE` | Facultatif, défaut `Standard_D4ls_v6`, à vérifier pour ta souscription |

Ajouter le secret **`TF_STATE_PASSPHRASE`**, aléatoire et long, au moins 16 caractères. Le conserver dans un gestionnaire de mots de passe. Les états et les plans OpenTofu sont chiffrés avec cette valeur. La même passphrase doit être utilisée lors des exécutions suivantes.

## Lancement

1. Pousser les fichiers sur `master` pour rendre les workflows disponibles.
2. Ouvrir `Actions → Infrastructure AKS → Run workflow` et sélectionner `apply`.
3. Le workflow crée le groupe `rg-mydisk-tfstate`, le compte Storage Standard LRS et son conteneur privé `tfstate`.
4. Il déplace son état initial chiffré dans ce stockage.
5. Il initialise les états Azure et Kubernetes, crée AKS et configure Traefik, le namespace et le Secret PostgreSQL.
6. Lancer ensuite le workflow `CD` pour publier les images et déployer MyDisk.

L’action `apply` crée des ressources payantes. Elle affiche puis applique ses plans dans la même exécution. L’action `plan` prévisualise uniquement AKS une fois le stockage initialisé. Pour exiger une approbation avant l’exécution du job, configurer les protections de l’environnement GitHub selon les fonctionnalités disponibles pour le dépôt.

Une attribution de rôle Azure peut mettre quelques minutes à devenir effective. En cas d’accès refusé juste après sa création, patienter puis relancer. Le bootstrap reprend les ressources déjà présentes dans le groupe dédié en cas d’interruption et utilise son état distant lorsqu’il existe. Si un bootstrap échoue, le workflow conserve son état local chiffré sept jours dans l’artefact `encrypted-bootstrap-recovery`, s’il a été créé.

## Organisation

| Dossier | Responsabilité |
|---|---|
| `infra/platform/azure/bootstrap` | Groupe et stockage d’état Azure |
| `infra/platform/azure` | Groupe applicatif et AKS |
| `infra/cluster` | Kubernetes et Helm via un contexte kubeconfig explicite |

Les déclarations de backend Azure sont générées dans la copie du dépôt utilisée par GitHub. La configuration Kubernetes peut donc utiliser un autre backend localement ou sur un autre cloud.

Le conteneur `tfstate` contient trois états, `mydisk-bootstrap.tfstate`, `mydisk-cluster-aks.tfstate` pour Azure et `mydisk-platform.tfstate` pour Kubernetes. Les deux derniers noms ont été conservés après réorganisation des dossiers pour retrouver les états existants.

Si les ressources sont déjà gérées par un état local, migrer cet état et conserver sa passphrase avant de lancer les workflows. Le bootstrap peut adopter un groupe de stockage créé manuellement. Si ce groupe est déjà géré par un autre état OpenTofu, transférer cet état au préalable pour conserver un propriétaire unique.

## Coûts et arrêt

Le défaut actuel est AKS Free avec un nœud. Vérifier l’éligibilité de la taille et son prix dans la souscription avant le premier lancement. Le plan OpenTofu ne calcule pas les prix. Les machines, disques, équilibreurs de charge et IP peuvent être facturés.

Après chaque séance, lancer **AKS start or stop**, action **stop**. Les ressources conservées, notamment les disques et le stockage d’état, peuvent rester facturées. L’action **start** remet le cluster en marche avant une livraison CD.

Pour une longue pause, lancer **Infrastructure AKS**, action **destroy**, puis saisir le nom du cluster pour confirmer. Cette action démarre au besoin AKS pour retirer les composants Kubernetes, puis détruit le cluster. Les données applicatives de cet environnement peuvent être perdues. Le stockage d’état est conservé.

## Déploiement applicatif

Le CD automatique sur `master` suppose un cluster créé et démarré. Le premier push peut échouer au déploiement avant le provisionnement. Relancer CD ensuite.

Les images GHCR doivent être accessibles à AKS. Utiliser des packages publics pour un portfolio, ou configurer `imagePullSecrets` pour des packages privés. Le jeton utilisé pour publier les images dans GitHub Actions ne donne pas automatiquement accès aux pods.
