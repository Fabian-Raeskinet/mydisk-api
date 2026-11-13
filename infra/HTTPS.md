# HTTPS for MyDisk

The Azure public IP of Traefik has the DNS label `mydisk-fabian`.
Application URL: https://mydisk-fabian.westeurope.cloudapp.azure.com/swagger/index.html

## Deploy in order

1. Publish these changes. If automatic CD starts before cert-manager is installed,
   its HTTPS prerequisite check fails before applying application resources.
2. Run **Infrastructure AKS**, action `apply`. This installs cert-manager and configures
   the Traefik Service annotations from `infra/platform/azure/ingress.tfvars`.
3. Run **CD** on the same commit. This applies the namespaced Let's Encrypt Issuer,
   TLS Ingress and HTTP redirect, then waits for the certificate and checks HTTPS.

The Azure-specific annotation stays in the Azure inputs. `infra/cluster` accepts
an arbitrary annotation map and uses only Kubernetes and Helm providers.
The actual DNS name in the cloud overlay must match the FQDN Azure assigns.
Changing the label requires updating the cloud Ingress hosts, Makefile and CD URL.

## Certificate lifecycle

cert-manager registers an ACME account without an email address, serves
HTTP-01 through temporary solver resources, and writes the certificate/private key
into Secret `mydisk-api-tls` in namespace `mydisk`. The key is not committed to Git.
Traefik terminates TLS and reloads renewed certificates. Keep public ports 80/443
accessible. The solver's more specific route takes precedence over the application's
HTTP redirect. There is no global entrypoint redirect.

The application talks HTTP internally. `Https__TerminatedAtProxy=true` disables its
own HTTPS redirect so HTTP health probes keep working. Application code requiring
the original client scheme or IP will also need trusted forwarded-header configuration.
HTTPS does not authenticate API callers or protect Swagger from public access.

For experimentation with ACME, use the staging directory URL in the Issuer:
https://acme-staging-v02.api.letsencrypt.org/directory
Staging certificates are intentionally untrusted: the CD HTTPS check must not be
bypassed to make that test pass. Restore the production URL for browser use.

## Diagnose

```sh
kubectl --context mydisk -n mydisk get issuer,certificate,certificaterequest,order,challenge
kubectl --context mydisk -n mydisk describe certificate mydisk-api-tls
curl -I http://mydisk-fabian.westeurope.cloudapp.azure.com/swagger/index.html
curl -I https://mydisk-fabian.westeurope.cloudapp.azure.com/swagger/index.html
```

Renewal runs in the cluster independently of GitHub Actions. A stopped cluster
cannot renew certificates. Deleting/recreating the cluster creates new keys and
certificate requests; avoid repeated issuance loops because CA rate limits apply.

## Local k3d HTTPS

Local URL: https://localhost:8443/swagger/index.html
HTTP on http://localhost:8080 redirects to this HTTPS port.

Local certificates use mkcert, not Let's Encrypt or cert-manager. No email or
public DNS is needed. The existing Traefik bundled with k3s terminates TLS.

One-time setup on a developer machine:

```sh
brew install mkcert  # only if missing
mkcert -install     # trusts the local development CA; macOS may request permission
make dev-up
```

`make dev-up` adds port 8443 to existing k3d load balancers without deleting the
cluster or its volumes. New clusters expose it from creation. `make k8s-init-tls`
generates a certificate for localhost, 127.0.0.1 and ::1 in the ignored `.tls/`
directory inside the local overlay. The local Kustomize overlay creates a TLS
Secret containing only the leaf certificate and leaf private key, never the CA key.
The certificate is reused while it has at least 30 days remaining; a later make
run regenerates it as needed. Unlike cert-manager, this is not background renewal.

For an existing, already running application, enable HTTPS without migrations:

```sh
make local-https
```

Verify with `curl -I https://localhost:8443/swagger/index.html`. If the browser
rejects the certificate, run `mkcert -install` locally and restart the browser.
Do not commit certificates/private keys or share mkcert's rootCA-key.pem.
