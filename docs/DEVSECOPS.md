# Understanding the security checks

CI audits NuGet dependencies, runs the tests, validates the infrastructure,
builds both images, scans them, and publishes them only if the checks pass.
CD depends on the successful completion of CI.

## 1. NuGet: audit dependencies across the solution

`Directory.Build.props` applies to every .NET project in the repository. It enables
NuGet Audit for direct and transitive dependencies, including test dependencies.
A transitive dependency is a library brought in by another library.

Low/Moderate findings remain warnings. High/Critical findings (NU1903/NU1904)
become errors. Failure to retrieve audit information (NU1900/NU1905) also blocks
the build: an unavailable audit is not a successful audit.
This NuGet policy blocks all High/Critical findings, even without a published fix.

Local command: `make security-dependencies`.
The log is saved to `security-reports/nuget-restore.txt`. When restore succeeds,
a JSON report is also generated. In CI, available reports are attached to the run
as `dependency-audit`, including after a failure.

The policy file is copied by the Dockerfile before restore, so Docker builds also
apply these checks. Image scanning may miss dependencies embedded in the EF bundle;
NuGet Audit covers their source dependency graph.

## 2. Trivy: scan the images that will be shipped

An image contains the application, its libraries, the .NET runtime, and Linux
components. Trivy inventories the components it recognizes and compares their
versions with a security advisory database. It does not simulate an attack or
prove either the exploitability of every finding or the absence of vulnerabilities.

CI builds the API and migrations images for linux/amd64, scans them in the runner's
local Docker engine, and runs `docker push` on those same images. There is no rebuild
between scanning and publication. Images remain tagged with the Git SHA as before;
this does not provide image signing or deployment by digest.

All commands are defined directly in the Makefile; no custom Python or shell
script is needed. The first Trivy invocation saves the complete JSON report,
including all severity levels and vulnerabilities without fixes. The second uses
Trivy's native `--severity HIGH,CRITICAL --ignore-unfixed --exit-code 1` options
to enforce the policy. Both inspect the same local image, with Trivy reusing its
cache. Scanner errors also block publication. The Makefile checks both images
even if the first fails and returns an overall failure when necessary.

For vulnerabilities without a fix, the image policy is less strict than the NuGet
policy because images contain distribution packages whose fixes may not yet be
available. Those findings still need review in the reports.

Reports are uploaded as `image-security-reports` artifacts and retained for 14 days.
This setup does not require SARIF uploads or a paid GitHub Code Security subscription.

Install Trivy once (`brew install trivy` on macOS) and start Docker. Then run:

```sh
make security
```

This runs the dependency audit, builds linux/amd64 images, then scans them in order.
It does not publish images, deploy resources, or run unit tests. Run `make test-unit`
for tests. The individual commands are also available:

```sh
make security-dependencies
make security-build
make security-images
```

GitHub Actions calls these same targets and uploads the generated reports.
Use Trivy 0.76.2 to match the current CI scanner version; newer installations may
produce different results. Advisory databases also change over time.

To scan other tags:

```sh
API_IMAGE=mydisk-api:my-tag MIGRATIONS_IMAGE=mydisk-api-migrations:my-tag make security-images
```

`make security-build` defaults to linux/amd64 on both Mac and CI, matching AKS.
You can override `SECURITY_PLATFORM`, but scanning a different architecture can
produce different results. Unlike `security-build`, the existing `make k8s-build`
uses the local Docker architecture for everyday local development.

## 3. Dependabot: propose updates

`.github/dependabot.yml` requests weekly checks for NuGet, Docker, and GitHub Actions,
with at most three open update PRs per ecosystem. These PRs are not merged
automatically. Their CI checks must pass, and their changes need review.
Dependabot security alerts are a separate GitHub setting; this file does not
guarantee that those alerts are enabled.

The new Trivy installation action is pinned to a commit SHA, and the scanner uses
an explicit version. This makes the tooling more reproducible. Existing actions
have not all been migrated to commit SHAs in this change.

## Verified remediation exercise

Before remediation, restoring the API failed with NU1903 for Microsoft.OpenApi
2.4.1 (GHSA-v5pm-xwqc-g5wc). A direct reference to version 2.7.5 replaces the
vulnerable transitive dependency. The advisory concerns parsing malicious OpenAPI
documents; the finding alone does not prove that Swagger in MyDisk exposes that
exploitation path.

The solution-wide audit also detected and led to fixes for these transitive
dependencies: System.Security.Cryptography.Xml/Pkcs 9.0.0 to 9.0.23 in the migrations
project, and Microsoft.Extensions.Caching.Memory 8.0.0 to 8.0.1 in the data test
utilities. Direct references make these fixes explicit. They can be removed once
the parent dependencies require patched versions themselves.

Trivy then detected two High vulnerabilities in AutoMapper 16.1.1
(CVE-2026-128081 and CVE-2026-128082) that were absent from the observed NuGet results.
Both AutoMapper references, in services and migrations, were updated to 16.3.0.
This difference illustrates the complementary coverage of advisory databases.

## When the pipeline fails

1. Open the first failing check and download its report.
2. Read the package name, installed version, GHSA/CVE advisory, and fixed version.
3. For NuGet, update the direct reference or the parent dependency.
4. For Linux/.NET components in the base image, rebuild with a patched base.
5. Run the audit, tests, and image scans again.

Do not disable scanning globally just to make the pipeline pass. This initial
version adds no exceptions. Any future exception should be reviewed, justified,
and time-limited.

## Scope and limitations

These checks do not replace API authentication, NetworkPolicies, Azure permissions,
backups, or updates to deployed resources. They do not yet scan Git secrets,
source code for security flaws (SAST), security manifests, or every third-party
image in the cluster, such as PostgreSQL, Traefik, and cert-manager.

This change does not add periodic scans of deployed images. A vulnerability
published after deployment requires another audit, rebuild, and deployment.
Existing manual publication commands are not blocked by CI; the guarantee described
here applies to the GitHub Actions workflow.

A CI failure blocks CD through `needs: ci`. To also prevent merging a PR, configure
required checks in the protection rules for `master`. Workflow YAML alone does
not configure those GitHub rules.

To try the checks without publishing images or deploying, run the CI workflow
manually (`workflow_dispatch`). It runs with `push-images` disabled.
