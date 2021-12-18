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

echo 'adding new tables...';

run_psql_script "tables/photo.category_role.sql";
run_psql_script "tables/video.category_role.sql";

echo 'seeding tables...';

run_psql_script "seed/photo.category_role.sql";
run_psql_script "seed/video.category_role.sql";

echo 'updating functions...';

run_psql_script "funcs/photo.get_categories.sql";
run_psql_script "funcs/photo.get_comments.sql";
run_psql_script "funcs/photo.get_gps.sql";
run_psql_script "funcs/photo.get_photo_metadata.sql";
run_psql_script "funcs/photo.get_photos.sql";
run_psql_script "funcs/photo.get_random_photos.sql";
run_psql_script "funcs/photo.get_ratings.sql";
run_psql_script "funcs/photo.get_years.sql";
run_psql_script "funcs/photo.save_comment.sql";
run_psql_script "funcs/photo.save_rating.sql";

run_psql_script "funcs/video.get_categories.sql";

echo "...${DBNAME} updated.";

# for podman - run as:
# ./update_idsrv.sh <pod_name> <path_to_pgbackup_env_file>
