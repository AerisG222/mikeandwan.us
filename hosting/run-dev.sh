#!/bin/bash

# postgres
podman run -dt --pod maw-pod -v maw-postgres:/var/lib/postgresql/data:rw,z postgres:12.2

# solr
podman run -dt --pod maw-pod -v maw-solr:/var/solr/data:rw,z maw-solr-dev

# photos
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z maw-photos-dev

# files
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z maw-files-dev

# auth
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z --env-file /home/mmorano/git/maw-auth.env maw-auth-dev

# api
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z -v maw-uploads:/maw-uploads:rw,z -v maw-images:/images:ro,z --env-file /home/mmorano/git/maw-api.env maw-api-dev

# www
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z -v maw-images:/images:ro,z -v maw-movies:/movies:ro,z --env-file /home/mmorano/git/maw-www.env maw-www-dev

# gateway
podman run -dt --pod maw-pod -v maw-certs:/certs:ro,z maw-gateway-dev