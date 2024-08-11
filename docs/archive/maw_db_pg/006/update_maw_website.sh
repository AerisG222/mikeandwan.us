#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating functions...';

run_psql_script "funcs/photo.get_categories.sql";
run_psql_script "funcs/video.get_categories.sql";

echo "...${DBNAME} updated.";
