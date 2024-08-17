#!/bin/bash
DBNAME="idsrv";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating functions...';
run_psql_script "funcs/idsrv.delete_persisted_grants.sql";

echo "...${DBNAME} updated.";
