#!/bin/bash

# postgres psql
podman run -it \
    --rm \
    --pod maw-pod \
    --name maw-psql \
    postgres:12.2 \
    psql -h localhost -U postgres

# remote archive
remote-archive/archive.sh
