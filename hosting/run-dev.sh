#!/bin/bash

# postgres
podman run -dt \
 --pod maw-pod \
    -v maw-postgres:/var/lib/postgresql/data:rw,z \
       postgres:12.2

# postgres psql
podman run -it \
 --pod maw-pod \
  --rm \
       postgres:12.2 \
       psql -h localhost -U postgres

# postgres maintenance
podman run -it \
 --pod maw-pod \
  --rm \
    -v maw-postgres-backup:/pg_backup:rw,z \
    --env-file /home/mmorano/git/maw-postgres-backup.env \
       maw-postgres-maintenance

# solr
podman run -dt \
 --pod maw-pod \
    -v maw-solr:/var/solr:rw,z \
       maw-solr-dev

# solr reindex
podman run -it \
 --pod maw-pod \
  --rm \
       maw-solr-reindex

# photos
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
       maw-photos-dev

# files
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
       maw-files-dev

# auth
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-auth-dataprotection:/dataprotection:rw,Z \
    -v maw-google-creds:/google-creds:ro,z \
    --env-file /home/mmorano/git/maw-auth.env \
       maw-auth-dev

# api
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-api-dataprotection:/dataprotection:rw,Z \
    -v maw-uploads:/maw-uploads:rw,z \
    -v /srv/www/website_assets/images:/maw-www/wwwroot/images:ro \
    --security-opt label=disable \
    --env-file /home/mmorano/git/maw-api.env \
       maw-api-dev

# www
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-www-dataprotection:/dataprotection:rw,Z \
    -v maw-google-creds:/google-creds:ro,z \
    -v /srv/www/website_assets/images:/maw-www/wwwroot/images:ro \
    -v /srv/www/website_assets/movies:/maw-www/wwwroot/movies:ro \
    --security-opt label=disable \
    --env-file /home/mmorano/git/maw-www.env \
       maw-www-dev

# gateway
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v /srv/www/website_assets:/assets:ro \
    --security-opt label=disable \
       maw-gateway-dev

# remote archive
remote-archive/archive.sh

# reverse geocode
podman run -it \
 --pod maw-pod \
    -v maw-reverse-geocode:/results:rw,z \
       maw-reverse-geocode \
       AUTO <conn_str> <api_key> /results
