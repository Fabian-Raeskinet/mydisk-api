# MyDisk use cases

MyDisk lets each account manage its own record collection and reviews. The `admin` role gives access to a screen for managing reviews across collections. This guide maps actions in the frontend to their HTTP routes and the components that handle them. [Architecture](ARCHITECTURE.md) · [Class responsibilities](CLASSES.md) · [Worker and moderation](WORKER.md)

## Actors and use cases

| Actor | Actions |
| --- | --- |
| Visitor | Sign in through Keycloak. The HTTP health probes are also available without a session. |
| Member | Manage a collection, submit and archive reviews, check review status, verify a session, and check API health. |
| MyDisk administrator | Use member features and view or archive reviews across collections with the `admin` client role. |
| Operator or monitoring client | Check health and maintenance status. |
| Developer or integrator | Explore the HTTP contract in Swagger. |
| Keycloak | Authenticate accounts, issue tokens, and end sessions when users sign out. |

```mermaid
flowchart LR
    V["Visitor"]
    M["Signed-in member"]
    A["MyDisk administrator"]
    O["Operator or monitoring client"]
    I["Developer or integrator"]
    K["Keycloak"]

    subgraph MyDisk
        S(["Sign in or out"])
        C(["Browse, filter, and sort a collection"])
        D(["Add, edit, or delete a record"])
        R(["Link an existing artist"])
        N(["Submit a review and track its moderation"])
        P(["Read and archive your own reviews"])
        Q(["View and archive reviews across collections"])
        U(["View your profile and verify your session"])
        H(["Check API health and maintenance status"])
        T(["Run the retry demo"])
        W(["Explore the HTTP contract in Swagger"])
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
    I --- W
    S --- K
```

The API identifies the owner by the pair of `iss` and `sub` claims in the validated token. Personal collection operations filter by that owner for every account, including administrators. The API treats resources outside the signed-in account's collection as missing. Legacy records with no owner stay outside personal collections. [Owner identity](../apps/api/src/Api/Controllers/DiskOwnerClaims.cs) · [Collection isolation tests](../apps/api/src/Api.IntegrationTests/Controllers/PrivateCollectionFixture.cs)

## HTTP routes

| Action | Method and route | Access | Successful response |
| --- | --- | --- | --- |
| Verify the token sent to the API | `GET /api/auth/me` | Valid token | `200` with claims |
| List records and their published reviews | `GET /api/Disk/disks` | Owner | `200` with a list, which may be empty |
| Look up a record by exact name | `GET /api/Disk/disk-by-name?name=…` | Owner | `200` with record details |
| Add a record | `POST /api/Disk/create-disk` | Session with `iss` and `sub` | `204` |
| Edit a record | `PUT /api/Disk/update-disk` | Owner | `204` |
| Delete a record | `DELETE /api/Disk/delete-disk?diskId=…` | Owner | `204` |
| Link an artist | `POST /api/Disk/attach-author` | Owner | `204` |
| Submit a review | `POST /api/Review` | Record owner | `202` with the review ID, `Pending` status, and a `Location` header |
| Look up a review by ID | `GET /api/Review/{reviewId}` | Record owner | `200` with review details |
| Archive a review from the record details | `POST /api/Review/Archive` | Record owner | `204` |
| List reviews across collections | `GET /api/admin/reviews` | `admin` role | `200` with reviews of every status |
| Archive a review from the administration screen | `POST /api/admin/reviews/{reviewId}/archive` | `admin` role | `204` |
| Check liveness, readiness, or overall health | `GET /health/live`, `GET /health/ready`, `GET /health` | Public | `200` with `Healthy` when the probe succeeds |
| Read maintenance status | `GET /maintenance` | Public | `200` with `Active` or `Inactive` |
| Run the retry demo | `GET /api/Miscellaneous/disks` | Valid token | `204` when the demo completes |
| Explore the HTTP contract | `GET /swagger/index.html` | Public | `200` with Swagger UI |

Protected routes return `401` when authentication fails and `403` when the token is valid and lacks the claims required for the operation. The MVC exception filter maps FluentValidation and format errors to `400` and missing resources to `404`. The maintenance filter returns `503` for the MVC actions it blocks. [Controllers](../apps/api/src/Api/Controllers/) · [JWT validation and Admin policy](../apps/api/src/Api/KeycloakAuthentication.cs) · [MVC filters](../apps/api/src/Api/Filters/)

## Sign in, verify the session, and sign out

All application pages in the frontend go through `RequireSession`. The local public Keycloak client uses the OIDC Authorization Code flow with PKCE S256. Protected API calls use an access token whose signature, expiration, issuer, and audience the API validates. The **Mon profil** page displays the Blazor session claims and can separately verify the claims in the access token sent to the API. [Blazor routing](../apps/frontend/src/App.razor) · [Local realm](../apps/keycloak/k8s/overlays/local/mydisk-realm.json) · [Profile page](../apps/frontend/src/Pages/ProfilePage.razor)

```mermaid
sequenceDiagram
    actor U as User
    participant B as Blazor frontend
    participant K as Keycloak
    participant A as API

    U->>B: Opens Ma collection
    B->>B: RequireSession checks for a session
    opt No session
        B->>K: OIDC redirect with PKCE challenge
        K-->>U: Sign-in form
        U->>K: Signs in
        K-->>B: Redirects to login-callback with the code
        B->>K: Exchanges the code with the PKCE verifier
        K-->>B: Session tokens
    end
    B-->>U: Opens the protected page
    U->>B: Verifies the API connection from Mon profil
    B->>A: GET /api/auth/me with Bearer token
    A->>A: Validates the token
    alt Token accepted
        A-->>B: 200 with validated claims
        B-->>U: Displays claims
    else Token rejected
        A-->>B: 401
        B-->>U: Displays the session error
    end
    opt Sign out
        U->>B: Requests sign-out
        B->>K: Ends the OIDC session
        K-->>B: Redirects to logout-callback
        B-->>U: Returns to the sign-in flow
    end
```

## Browse the collection and open record details

The **Ma collection** page filters records in the browser by name or artist pseudonym and sorts by name or release date. The **Avis** page uses the same private collection and shows its published reviews. A record's detail view also shows pending and rejected reviews. Archived reviews are omitted from record details. The API looks up a record by exact name. If multiple records share a name and the returned ID differs from the user's selection, the frontend displays an ambiguity message. [Navigation and loading](../apps/frontend/src/Components/CollectionWorkspace.razor.cs) · [EF Core repository](../apps/api/src/Data/DiskRepository.cs) · [Record mapping](../apps/api/src/Services/Disks/MapperProfiles.cs)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as CollectionWorkspace
    participant A as DiskController
    participant H as Query handlers
    participant P as PostgreSQL

    U->>B: Opens Ma collection or Avis
    B->>A: GET /api/Disk/disks
    A->>H: GetAllDisksQueryRequest with owner
    H->>P: Reads the member's records, artists, and published reviews
    P-->>H: Collection results
    H-->>A: List of DiskResult
    A-->>B: 200 with the collection
    B-->>U: Displays records or published reviews
    U->>B: Opens a record's details
    B->>A: GET /api/Disk/disk-by-name with the exact name
    A->>H: GetDiskByNameQueryRequest with owner
    H->>P: Looks up the record and loads its reviews
    alt Record found
        P-->>H: Record and reviews
        H-->>A: DiskResult with unarchived reviews
        A-->>B: 200 with details
        alt ID matches the selection
            B-->>U: Displays record details
        else ID differs
            B-->>U: Reports the ambiguous name
        end
    else Record missing
        A-->>B: 404
        B-->>U: Displays the error
    end
```

## Add, edit, or delete a record

The Blazor form asks for a name of up to 30 characters and a release date. On creation, the server reads the owner from the token and raises a `NewDiskCreatedDomainEvent`. Its handler logs the record ID during the EF Core save. The update endpoint applies the supplied fields. Deletion requires confirmation in the frontend and cascades to the record's reviews. [Blazor editor](../apps/frontend/src/Components/DiskEditor.razor.cs) · [Record commands](../apps/api/src/Services/Disks/) · [Record-to-review relationship](../apps/api/src/Data.Configurations/Reviews/ReviewConfiguration.cs)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as CollectionWorkspace
    participant A as DiskController
    participant C as MediatR pipeline and handler
    participant P as PostgreSQL

    U->>B: Submits the form or confirms deletion
    alt Add
        B->>A: POST /api/Disk/create-disk
        A->>C: CreateDiskCommandRequest with iss and sub
        C->>C: Creates the record and adds its creation event
        C->>P: Saves the record
    else Edit
        B->>A: PUT /api/Disk/update-disk
        A->>C: UpdateDiskCommandRequest with owner
        C->>P: Loads the record by ID and owner
        C->>P: Saves the supplied fields
    else Delete
        B->>A: DELETE /api/Disk/delete-disk
        A->>C: DeleteDiskCommandRequest with owner
        C->>P: Loads the record by ID and owner
        C->>P: Deletes the record and its reviews
    end
    alt Operation succeeds
        C-->>A: Write complete
        A-->>B: 204
        B->>A: Reloads the collection and any open details
        B-->>U: Displays confirmation
    else Record to edit or delete is missing from the collection
        C-->>A: ObjectNotFoundException
        A-->>B: 404
        B-->>U: Displays the error
    end
```

## Link an existing artist

In the record details, the member selects an artist from the loaded collection or enters the artist's UUID. The server loads the record owned by the account and the matching artist. When the record has a release date, `Disk.AttachAuthor` requires the artist's birth date to precede it. [Artist form](../apps/frontend/src/Components/DiskDetail.razor.cs) · [Handler](../apps/api/src/Services/Disks/AttachAuthorCommandHandler.cs) · [Domain rule](../apps/api/src/Domain/Disks/Disk.cs)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as CollectionWorkspace
    participant A as DiskController
    participant H as AttachAuthorCommandHandler
    participant P as PostgreSQL

    U->>B: Picks an artist or enters the artist's UUID
    B->>A: POST /api/Disk/attach-author
    A->>H: AttachAuthorCommandRequest with owner
    H->>P: Looks up the record by ID and owner, then the artist by ID
    alt Both resources found and dates are valid
        H->>H: Disk.AttachAuthor
        H->>P: Saves the artist link
        A-->>B: 204
        B->>A: Reloads the collection and record details
    else Either resource is missing
        A-->>B: 404
        B-->>U: Displays the error
    else Dates violate the domain rule
        H-->>A: InvalidOperationException
        A-->>B: 500
        B-->>U: Displays the error
    end
```

## Submit a review and track its status

The Blazor record details show the form from the record's release date. The form collects a title of up to 30 characters, the review text, and a rating from 0 to 5. The API saves the `Pending` review and its outbox message in the same transaction, then returns `202`. The frontend polls by review ID with increasing, jittered delays and checks a 60-second deadline between attempts. Polling stops when the review leaves `Pending` or the API returns `404`. Reopening the record details resumes tracking any pending reviews. [Blazor record details](../apps/frontend/src/Components/DiskDetail.razor) · [Controller](../apps/api/src/Api/Controllers/ReviewController.Post.cs) · [Frontend polling](../apps/frontend/src/Services/ReviewModerationWatcher.cs) · [Message publishing and consumption](WORKER.md)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as Blazor frontend
    participant A as ReviewController
    participant M as Services and EF Core
    participant P as PostgreSQL
    participant W as Moderation worker

    U->>B: Submits the review form
    B->>A: POST /api/Review
    A->>M: CreateReviewCommandRequest with owner
    M->>M: Validates the command
    opt Command is valid
        M->>P: Looks up the record by ID and owner
    end
    alt Command is valid and record exists
        M->>M: Adds a Pending review and raises its creation event
        M->>P: Saves the review and outbox entry in one transaction
        A-->>B: 202 with ID, Pending status, and Location
        B-->>U: Shows that the review is awaiting moderation
        Note over P,W: The worker processes the message asynchronously
        loop Until a decision, 404, or the polling deadline
            B->>A: GET /api/Review/{reviewId}
            A->>M: GetReviewQueryRequest with owner
            M->>P: Looks up the review by ID and record owner
            P-->>M: Current status or review missing
            A-->>B: 200 with status or 404
        end
        opt Decision received or deadline reached
            B-->>U: Updates the status or reports the delay
        end
    else Record missing or command invalid
        A-->>B: 404 or 400
        B-->>U: Displays the error
    end
```

## Archive a review as the owner

Archiving from the record details uses the same ownership check as a direct review lookup. It sets the review's status to `Archived`. On reload, the review disappears from the record details and the published reviews list. The owner can still retrieve it by ID, and it remains visible on the administration screen. [Archive handler](../apps/api/src/Services/Reviews/ArchiveReviewCommandHandler.cs) · [Query handler](../apps/api/src/Services/Reviews/GetReviewQueryHandler.cs) · [Record mapping](../apps/api/src/Services/Disks/MapperProfiles.cs)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as Record details
    participant A as ReviewController
    participant H as ArchiveReviewCommandHandler
    participant P as PostgreSQL

    U->>B: Archives their review
    B->>A: POST /api/Review/Archive with reviewId
    A->>H: ArchiveReviewCommandRequest with owner
    H->>P: Looks up the review by ID and record owner
    alt Review found
        H->>H: Review.Archive
        H->>P: Saves the Archived status
        A-->>B: 204
        B->>A: Reloads the collection and record details
        B-->>U: Confirms the review was archived
    else Review missing
        A-->>B: 404
        B-->>U: Displays the error
    end
```

## Administer reviews

The frontend shows **Administration → Avis** when the account's `mydisk_roles` claim contains `admin`. The controller enforces the `Admin` policy on the JWT and also requires `sub`. The list includes reviews of every status with their record names. When a review is archived for the first time, the controller logs the issuer, subject, and review ID after the write. Repeating the operation on an archived review also returns `204` without another review update. [Administration screen](../apps/frontend/src/Components/AdminReviewsView.razor) · [Controller](../apps/api/src/Api/Controllers/AdminReviewsController.cs) · [Handler](../apps/api/src/Services/Reviews/ArchiveReviewAsAdminCommand.cs)

```mermaid
sequenceDiagram
    actor U as Administrator
    participant B as AdminReviewsView
    participant A as API with Admin policy
    participant H as Admin handlers
    participant P as PostgreSQL

    U->>B: Opens Administration, then Avis
    B->>A: GET /api/admin/reviews
    alt Admin policy satisfied
        A->>H: GetAdminReviewsQuery
        H->>P: Loads reviews across collections with their records
        P-->>H: Reviews of every status
        A-->>B: 200 with the list
        U->>B: Confirms removal of a review
        B->>A: POST /api/admin/reviews/{reviewId}/archive
        A->>A: Applies the Admin policy again
        A->>H: ArchiveReviewAsAdminCommand
        H->>P: Looks up the review by ID
        alt Review can be archived
            H->>P: Sets the review to Archived
            A->>A: Logs the archive action
            A-->>B: 204
            B-->>U: Displays the archived status
        else Review already archived
            A-->>B: 204
        else Review missing
            A-->>B: 404
        end
    else Token missing or invalid, or policy check fails
        A-->>B: 401 or 403
    end
```

## Health probes, maintenance, and retry demo

The **État de l’API** screen calls the three health probes and `/maintenance` in parallel. `/health/live` confirms that the HTTP process responds. `/health/ready` checks PostgreSQL when a connection is configured, and `/health` runs all registered checks. Setting `MaintenanceSettings:Global` blocks MVC actions covered by the maintenance filter. The probes, `/maintenance`, and the `/api/auth/me` minimal API route remain available under their usual access rules. [Probes and routes](../apps/api/src/Api/Program.cs) · [PostgreSQL health check registration](../apps/api/src/Api/DependencyInjection.cs) · [API status screen](../apps/frontend/src/Components/Diagnostics.razor.cs)

```mermaid
sequenceDiagram
    actor U as Member
    participant B as Diagnostics
    participant A as API
    participant P as PostgreSQL
    participant R as RetryServiceHandler and Polly

    U->>B: Opens État de l’API
    par Liveness
        B->>A: GET /health/live
        A-->>B: Liveness result
    and Readiness and overall health
        B->>A: GET /health/ready and GET /health
        opt PostgreSQL check configured
            A->>P: Checks connectivity
            P-->>A: Connectivity result
        end
        A-->>B: Probe results
    and Maintenance
        B->>A: GET /maintenance
        A-->>B: 200 with Active or Inactive
    end
    B-->>U: Displays the results
    opt Run the retry demo
        U->>B: Runs the retry demo
        B->>A: GET /api/Miscellaneous/disks with Bearer token
        A->>R: RetryServiceRequest
        R->>R: Tries a result-based policy, then an exception-based policy
        A-->>B: 204 if the demo completes
        B-->>U: Displays the outcome
    end
```

The demo uses `PollyRetryService` to run two scenarios driven by random values. `RetryServiceSettings` configures the retry count and delay. The worker guide covers the retry behavior of the moderation flow. [Demo scenarios](../apps/api/src/Services/Miscellaneous/RetryServiceHandler.cs) · [Polly implementation](../apps/api/src/RetryService/PollyRetryService.cs) · [Worker retries](WORKER.md)
