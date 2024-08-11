#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'altering tables...';
run_psql_script "tables/photo.category.sql";
run_psql_script "tables/photo.photo.sql";
run_psql_script "tables/video.category.sql";
run_psql_script "tables/video.video.sql";
run_psql_script "tables/video.comment.sql";
run_psql_script "tables/video.rating.sql";

echo 'running seed...'
run_psql_script "seed/aws.glacier_vault.sql";

echo "...${DBNAME} updated.";
