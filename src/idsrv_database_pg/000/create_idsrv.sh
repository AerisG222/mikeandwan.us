#!/bin/bash
DBNAME="idsrv";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo "creating database ${DBNAME}...";
database/idsrv.sh "${DBNAME}";

echo 'creating roles...';
run_psql_script "roles/idsrv.sql";

echo 'creating users...';
run_psql_script "users/svc_www_maw.sql";

echo 'creating schemas...';
run_psql_script "schemas/idsrv.sql";

echo 'creating tables...';
run_psql_script "tables/idsrv.persisted_grant.sql";

echo "...${DBNAME} created.";

echo "If there were errors, you might need to run this script as a user with superuser privileges.";
echo "To do so, as the postgres OS user, run psql, then 'CREATE USER <username> WITH SUPERUSER;'";
