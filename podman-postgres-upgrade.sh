#!/bin/bash

POD="maw-pod-dev"

# we use the same names below for both containers and volumes
# note: at the end we will reuse the same container name so that the systemd startup scripts continue to work
PG_OLD="maw-postgres"
PG_OLD_IMG="postgres:12.2"
PG_NEW="maw-postgres.13.2"
PG_NEW_IMG="postgres:13.2-alpine"
PG_ENV_FILE="/home/mmorano/podman-env-files/maw-postgres.env"
PG_DUMP_FILE="__pg.dump"
LOCALHOST_IP="$(hostname -I | cut -d ' ' -f 1)"

## note: envfile should have POSTGRES_PASSWORD and PGPASSWORD set to same value in the env file.  this allows
##       for the new pg container to set the proper superuser password, and the second is to allow for client
##       utilities to supply this password in the scripts below

source ./podman-shared.sh

# 1. stop the pod to avoid chance of new data being entered/lost
echo 'stopping pod...'
podman pod stop "${POD}"
echo 'pod stopped'
echo

# 2. start only our sql container
echo 'starting OLD sql container...'
podman start "${PG_OLD}"
echo 'started'
echo

# 3. create new volume if needed
echo 'creating new volume if required...'
create_volume "${PG_NEW}"
echo

# 4. start an instance of the new container and expose port 5432 to host on 9432
echo 'starting NEW sql container for migration purposes...'
podman run \
    -d \
    --rm \
    --name "${PG_NEW}" \
    --env-file "${PG_ENV_FILE}" \
    -p 9432:5432 \
    -v "${PG_NEW}":/var/lib/postgresql/data \
    "${PG_NEW_IMG}"
echo 'started'
echo

# 5. dump databases from old
echo 'starting to dump databases (pg_dumpall)...'
podman run \
    --rm \
    --pod "${POD}" \
    --env-file "${PG_ENV_FILE}" \
    "${PG_OLD_IMG}" \
    pg_dumpall -h localhost -U postgres -w \
    > "${PG_DUMP_FILE}"
echo 'dump created'
echo

# 6. restore databases to new
echo 'starting to restore databases'
podman run \
    -i \
    --name "new-pgrestore" \
    --rm \
    --env-file "${PG_ENV_FILE}" \
    "${PG_NEW_IMG}" \
    psql -h "${LOCALHOST_IP}" -p 9432 -U postgres -w \
    < "${PG_DUMP_FILE}"
echo 'migration complete!'
echo

# 7. optimize databases after migration
echo 'optimizing...'
podman run \
    --rm \
    --env-file "${PG_ENV_FILE}" \
    "${PG_NEW_IMG}" \
    reindexdb -h "${LOCALHOST_IP}" -p 9432 -U postgres -a -w && vacuumdb -h "${LOCALHOST_IP}" -p 9432 -U postgres --all --analyze -w
echo 'optimization complete'
echo

# 8. removing dump temp file
rm "${PG_DUMP_FILE}"

# 9. verify by querying data
echo 'verify data via sql...'
podman run \
    -it \
    --rm \
    --env-file "${PG_ENV_FILE}" \
    "${PG_NEW_IMG}" \
    psql -h "${LOCALHOST_IP}" -p 9432 -U postgres -w
echo 'verification complete'
echo

# 10. stop the new pg container
echo 'stopping / removing temp migration container...'
podman stop "${PG_NEW}"
echo 'stopped';
echo

DO_DESTROY="N"
read -e -r -p "REMOVE OLD / ENABLE NEW?? (only do this if data validation looks good) [N/y]" DO_DESTROY

if [ ! "${DO_DESTROY}" = "y" ]; then
    echo "Exiting without running cleanup"
    exit
fi;

# 11. stop pod
echo 'stopping pod...'
podman pod stop "${POD}"
echo 'pod stopped'
echo

# 12. remove old container
echo 'removing old container...'
podman rm "${PG_OLD}"
echo 'removed'
echo

# 13. create new container to replace prior
echo 'creating new container (with old name for systemd)...'
podman create \
    --pod "${POD}" \
    --name "${PG_OLD}" \
    --volume "${PG_NEW}":/var/lib/postgresql/data \
    "${PG_NEW_IMG}"
echo 'created'
echo

# 14. start pod
echo 'starting pod'
podman pod start "${POD}"
echo 'pod started'
echo

DO_DESTROY="N"
read -e -r -p "DELETE OLD DATA?? (only do this if sites work as expected) [N/y]" DO_DESTROY

if [ ! "${DO_DESTROY}" = "y" ]; then
    echo "Exiting without deleting old volume"
    exit
fi;

# 15. delete old volume/data
echo 'deleting old volume...'
podman volume rm "${PG_OLD}"
echo 'deleted.'
echo
