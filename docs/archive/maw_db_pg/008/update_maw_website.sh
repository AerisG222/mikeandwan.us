#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating readonly role...';
run_psql_script "roles/readonly.sql";

echo 'creating svc_solr user...'
run_psql_script "users/svc_solr.sql";

echo "...${DBNAME} updated.";

echo ""
echo "Please update the password for svc_solr"
echo ""
