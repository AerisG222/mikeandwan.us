#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating new tables...';
run_psql_script "tables/photo.gps_override.sql";
run_psql_script "tables/video.gps_override.sql";

echo 'updating tables...';
run_psql_script "tables/photo.point_of_interest.sql";
run_psql_script "tables/photo.reverse_geocode.sql";

run_psql_script "tables/video.point_of_interest.sql";
run_psql_script "tables/video.reverse_geocode.sql";

echo 'creating/updating functions...'
run_psql_script "funcs/photo.get_photos.sql";
run_psql_script "funcs/photo.get_gps.sql";
run_psql_script "funcs/photo.set_gps_override.sql";

run_psql_script "funcs/video.get_videos.sql";
run_psql_script "funcs/video.get_gps.sql";
run_psql_script "funcs/video.set_gps_override.sql";

echo "...${DBNAME} updated.";
