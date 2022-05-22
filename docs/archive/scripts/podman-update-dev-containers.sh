#!/bin/bash
POD=maw-pod-dev

# do not use this to upgrade postgres versions!

podman pod stop "${POD}"

# delete all containers
podman rm maw-photos
podman rm maw-files
podman rm maw-gateway
podman rm maw-auth
podman rm maw-api
podman rm maw-www
podman rm maw-solr
podman rm maw-postgres

# recreate dev containers
./podman-build-pod.sh dev
