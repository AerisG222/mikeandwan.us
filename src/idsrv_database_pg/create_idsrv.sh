#!/bin/bash
DBNAME="idsrv"
PODNAME=$1
ENVFILE=$2

function header() {
    echo "*** ${1} ***"
}

function run_psql_script() {
    if [ "${PODNAME}" == "" ]; then
        psql -d "${DBNAME}" -q -f "$1";
    else
        podman run -it --rm \
            --pod "${PODNAME}" \
            --env-file "${ENVFILE}" \
            --volume "$(pwd)":/tmp/context:ro \
            --security-opt label=disable \
            postgres:16-alpine \
                psql \
                    -h 127.0.0.1 \
                    -U postgres \
                    -d "${DBNAME}" \
                    -q \
                    -f "/tmp/context/${1}"
    fi
}

# header "database ${DBNAME}"
# database/create.sh "${DBNAME}"

# pgcrypto required for uuidv7 func below
header "extensions"
run_psql_script "extensions/pgcrypto.sql"

header "roles"
run_psql_script "roles/idsrv.sql"

header "users"
run_psql_script "users/svc_www_maw.sql"

header "schemas"
run_psql_script "schemas/idsrv.sql"

header "common functions"
run_psql_script "functions/uuid_generate_v7.sql"

header "pre-migration"
#...

header "tables"
#...

header "post-migration"
#...

header "seed"
#...

header "functions"
#...

header "completed ${DBNAME}"
