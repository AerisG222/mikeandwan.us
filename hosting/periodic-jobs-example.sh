#!/bin/bash

# postgres psql
podman run -it \
    --rm \
    --pod maw-pod \
    --name maw-psql \
    postgres:12.2 \
    psql -h localhost -U postgres

# postgres maintenance
podman run -it \
    --rm \
    --pod maw-pod \
    --name maw-postgres-maintenance \
    --volume maw-postgres-backup:/pg_backup:rw,z \
    --env-file /home/mmorano/git/maw-postgres-backup.env \
    maw-postgres-maintenance

# solr reindex
podman run -it \
    --rm \
    --pod maw-pod \
    --name maw-solr-reindex \
    maw-solr-reindex

# remote archive
remote-archive/archive.sh

# reverse geocode
podman run -it \
    --pod maw-pod \
    --name maw-reverse-geocode \
    --volume maw-reverse-geocode:/results:rw,z \
    maw-reverse-geocode \
    AUTO <conn_str> <api_key> /results
