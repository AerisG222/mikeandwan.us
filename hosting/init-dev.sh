#!/bin/bash

# create pod
podman pod create --name maw-pod -p 8080:80 -p 8443:443

# create volumes
podman volume create maw-certs
podman volume create maw-postgres
podman volume create maw-postgres-backups
podman volume create maw-solr
podman volume create maw-uploads
podman volume create maw-api-dataprotection
podman volume create maw-auth-dataprotection
podman volume create maw-www-dataprotection
podman volume create maw-google-creds
podman volume create maw-gdrive-creds
podman volume create maw-reverse-geocode

# init certs
podman run -it --rm -v maw-certs:/certs:rw,z maw-certs-dev

# trust dev ca on local machine
CERT_MOUNT_POINT=$(podman volume inspect maw-certs | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
sudo find "${CERT_MOUNT_POINT}/internal/ca" -type f -name *.crt -exec cp {} /usr/share/pki/ca-trust-source/anchors/ \;
sudo update-ca-trust

# copy google creds to volume
GOOGLECRED_VOL_MOUNT_POINT=$(podman volume inspect maw-google-creds | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
cp ~/www_emailer.json "${GOOGLECRED_VOL_MOUNT_POINT}"

# copy solr config+db from current system to volume, then configure permissions for container
SOLR_VOL_MOUNT_POINT=$(podman volume inspect maw-solr | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
SOLR_VOL_MOUNT_ROOT=$(dirname "${SOLR_VOL_MOUNT_POINT}")

sudo cp -R /var/solr/data "${SOLR_VOL_MOUNT_POINT}"
sudo chown -R "${USER}":"${USER}" "${SOLR_VOL_MOUNT_ROOT}"
sudo chcon -R unconfined_u:object_r:container_file_t:s0 "${SOLR_VOL_MOUNT_POINT}"
podman unshare chown -R 8983:8983 "${SOLR_VOL_MOUNT_ROOT}"

# test solr
# podman run -it --rm -v maw-solr:/var/solr/data:rw,z -p 8983:8983 maw-solr-dev

# copy posgtres db from current system to volume
# first - please make sure you know the postgres / admin password.  if you don't log in to the current db and set one with: alter user postgres with password 'NEW_PASSWORD';
PGSQL_VOL_MOUNT_POINT=$(podman volume inspect maw-postgres | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
PGSQL_VOL_MOUNT_ROOT=$(dirname "${PGSQL_VOL_MOUNT_POINT}")

sudo cp -a /var/lib/pgsql/data/. "${PGSQL_VOL_MOUNT_POINT}"
sudo chown -R "${USER}":"${USER}" "${PGSQL_VOL_MOUNT_ROOT}"
sudo chcon -R unconfined_u:object_r:container_file_t:s0 "${PGSQL_VOL_MOUNT_POINT}"
podman unshare chown -R 999:999 "${PGSQL_VOL_MOUNT_ROOT}"

# test postgres
# podman run -it --rm -v maw-postgres:/var/lib/postgresql/data:rw,z -p 5432:5432 postgres:12.2

# gdrive (offsite backup)
# follow instructions [here](https://hub.docker.com/r/d0whc3r/gdrive) to get credentials.json file
GDRIVE_VOL_MOUNT_POINT=$(podman volume inspect maw-gdrive-creds | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
GDRIVE_VOL_MOUNT_ROOT=$(dirname "${GDRIVE_VOL_MOUNT_POINT}")

mv credentials.json "${GDRIVE_VOL_MOUNT_POINT}"
chcon -R unconfined_u:object_r:container_file_t:s0 "${GDRIVE_VOL_MOUNT_POINT}"
podman run --rm -it -v maw-gdrive-creds:/app/secrets/ d0whc3r/gdrive -l
# follow prompt to view the google consent screen, then copy the code to the terminal to finalize getting the token
# if everything worked, you should see a list of files from gdrive
