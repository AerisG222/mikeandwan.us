#!/bin/bash
podman run -it --rm \
    --pod dev-maw-pod \
    --env-file ~/maw_dev/podman-env/maw-postgres.env \
    docker.io/postgres:16-alpine psql -h localhost -U postgres -d maw_website

