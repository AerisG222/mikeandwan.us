#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'altering tables...';
run_psql_script "tables/maw.user.sql";

echo "...${DBNAME} updated.";
