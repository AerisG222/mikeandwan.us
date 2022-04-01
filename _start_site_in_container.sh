#!/bin/bash
set -e

# trust the self signed CA certs (fedora)
#cp /certs/internal/ca/ca_*.crt /usr/share/pki/ca-trust-source/anchors/
#update-ca-trust

# trust the self signed CA certs (Alpine)
# credit: https://hackernoon.com/alpine-docker-image-with-secured-communication-ssl-tls-go-restful-api-128eb6b54f1f
cp /certs/ca/ca_*.crt /usr/local/share/ca-certificates
update-ca-certificates

exec "$@"
