#!/bin/bash
DBNAME="idsrv";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating functions...';
run_psql_script "funcs/idsrv.delete_persisted_grants.sql";
run_psql_script "funcs/idsrv.get_persisted_grants.sql";
run_psql_script "funcs/idsrv.save_persisted_grant.sql";

echo "...${DBNAME} updated.";
