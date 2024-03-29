#!/bin/bash
DBNAME="maw_website";
PODNAME=$1
ENVFILE=$2

function run_psql_script() {
    if [ "${PODNAME}" == "" ]; then
        psql -d "${DBNAME}" -q -f "$1";
    else
        podman run -it --rm --pod "${PODNAME}" --env-file "${ENVFILE}" -v "$(pwd)":/tmp/context:ro --security-opt label=disable postgres:12.2 psql -h 127.0.0.1 -U postgres -d "${DBNAME}" -q -f "/tmp/context/${1}"
    fi
}

echo 'seeding tables...';

run_psql_script "seed/google_demo_photo_categories.sql";

echo "...${DBNAME} updated.";

# for podman - run as:
# ./update_idsrv.sh <pod_name> <path_to_pgbackup_env_file>
