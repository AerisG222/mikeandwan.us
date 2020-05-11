#!/bin/bash
set -e

# trust the self signed CA certs
cp /certs/internal/ca/ca_*.crt /usr/share/pki/ca-trust-source/anchors/
update-ca-trust

exec "$@"
