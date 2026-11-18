#!/usr/bin/env bash
set -euo pipefail
tls_dir=$1
shift
command -v mkcert >/dev/null || { echo 'Install mkcert, then run mkcert -install.' >&2; exit 1; }
umask 077
mkdir -p "$tls_dir"
cert="$tls_dir/tls.crt"
key="$tls_dir/tls.key"
if [ -s "$cert" ] && [ -s "$key" ] &&
  openssl x509 -checkend 2592000 -noout -in "$cert" >/dev/null 2>&1 &&
  openssl verify -CAfile "$(mkcert -CAROOT)/rootCA.pem" "$cert" >/dev/null 2>&1; then
  echo "Reusing the local HTTPS certificate for $1."
else
  mkcert -cert-file "$cert" -key-file "$key" "$@"
fi
