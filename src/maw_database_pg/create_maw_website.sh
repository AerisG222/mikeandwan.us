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

# header "database ${DBNAME}"
# database/create.sh "${DBNAME}"

# pgcrypto required for uuidv7 func below
header "extensions"
run_psql_script "extensions/pgcrypto.sql"

header "roles"
run_psql_script "roles/website.sql"

header "users"
run_psql_script "users/svc_www_maw.sql"

header "schemas"
run_psql_script "schemas/aws.sql"
run_psql_script "schemas/blog.sql"
run_psql_script "schemas/maw.sql"
run_psql_script "schemas/media.sql"

header "common functions"
run_psql_script "functions/uuid_generate_v7.sql"

header "temp cleanup  ** REMOVE WHEN FINAL **"
run_psql_script "tables/temp_cleanup.sql"

header "pre-migration"
run_psql_script "_migrations/2024/pre_create/blog.drop_functions.sql"
run_psql_script "_migrations/2024/pre_create/blog.drop_fks.sql"
run_psql_script "_migrations/2024/pre_create/blog.blog.sql"
run_psql_script "_migrations/2024/pre_create/blog.post.sql"

header "tables"
run_psql_script "tables/blog.blog.sql"
run_psql_script "tables/blog.post.sql"
run_psql_script "tables/media.type.sql"
run_psql_script "tables/media.category.sql"
run_psql_script "tables/media.category_role.sql"
run_psql_script "tables/media.media.sql"

header "post-migration"
run_psql_script "_migrations/2024/post_create/blog.blog.sql"
run_psql_script "_migrations/2024/post_create/blog.post.sql"
run_psql_script "_migrations/2024/post_create/blog.drop_old.sql"

header "seed"
run_psql_script "seed/media.type.sql"

header "functions"
run_psql_script "functions/blog.add_post.sql"
run_psql_script "functions/blog.get_blogs.sql"
run_psql_script "functions/blog.get_posts.sql"

header "migrations"
#run_psql_script "tables/media.category.sql"

header "completed ${DBNAME}"
