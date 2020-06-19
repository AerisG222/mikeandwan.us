#!/bin/bash
DBNAME="idsrv";
PODNAME=$1
ENVFILE=$2

function run_psql_script() {
    if [ "${PODNAME}" == "" ]; then
        psql -d "${DBNAME}" -q -f "$1";
    else
        podman run -it --rm --pod "${PODNAME}" --env-file "${ENVFILE}" -v "$(pwd)":/tmp/context:ro --security-opt label=disable postgres:12.2 psql -h 127.0.0.1 -U postgres -d idsrv -f "/tmp/context/${1}"
    fi
}

echo 'updating tables...';
run_psql_script "tables/idsrv.persisted_grant.sql";

echo 'creating / updating functions...';
run_psql_script "funcs/idsrv.delete_persisted_grant.sql";
run_psql_script "funcs/idsrv.delete_persisted_grants.sql";
run_psql_script "funcs/idsrv.get_persisted_grant.sql";
run_psql_script "funcs/idsrv.get_persisted_grants.sql";
run_psql_script "funcs/idsrv.save_persisted_grant.sql";

echo "...${DBNAME} updated.";

# for podman - run as:
# ./update_idsrv.sh <pod_name> <path_to_pgbackup_env_file>
