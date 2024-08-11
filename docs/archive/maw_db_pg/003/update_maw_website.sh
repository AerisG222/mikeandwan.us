#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'altering tables...';
run_psql_script "tables/maw.login_history.sql";
run_psql_script "tables/maw.user.sql";
run_psql_script "tables/photo.photo.sql";
run_psql_script "tables/photo.raw_conversion_mode.sql";

echo "...${DBNAME} updated.";
