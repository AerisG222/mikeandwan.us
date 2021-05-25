#!/bin/bash

podman stop maw-postgres

podman create \
    --pod maw-pod \
    --name maw-postgres \
    --volume maw-postgres.13.2:/var/lib/postgresql/data:rw,z \
    docker.io/postgres:13.3

podman auto-update
