#!/bin/bash
DBNAME="maw_website"
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

header "database ${DBNAME}"
database/create.sh "${DBNAME}"

header "roles"
run_psql_script "roles/website.sql"

header "users"
run_psql_script "users/svc_www_maw.sql"

header "schemas"
run_psql_script "schemas/aws.sql"
run_psql_script "schemas/blog.sql"
run_psql_script "schemas/maw.sql"
run_psql_script "schemas/media.sql"

header "tables"
run_psql_script "tables/media.type.sql"
run_psql_script "tables/media.category.sql"
run_psql_script "tables/media.media.sql"

header "seed"
run_psql_script "seed/media.type.sql"

header "migrations"

header "completed ${DBNAME}"
