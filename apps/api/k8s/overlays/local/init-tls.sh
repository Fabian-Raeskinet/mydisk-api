#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
command -v mkcert >/dev/null || { echo 'Install mkcert, then run mkcert -install.' >&2; exit 1; }
umask 077
mkdir -p .tls
if [ -s .tls/tls.crt ] && [ -s .tls/tls.key ] && openssl x509 -checkend 2592000 -noout -in .tls/tls.crt >/dev/null 2>&1; then
  echo 'Reusing the local HTTPS certificate.'
else
  mkcert -cert-file .tls/tls.crt -key-file .tls/tls.key localhost 127.0.0.1 ::1
fi
