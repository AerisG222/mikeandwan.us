#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating new tables...';
run_psql_script "tables/photo.point_of_interest.sql";
run_psql_script "tables/photo.reverse_geocode.sql";
run_psql_script "tables/video.point_of_interest.sql";
run_psql_script "tables/video.reverse_geocode.sql";

echo "...${DBNAME} updated.";
