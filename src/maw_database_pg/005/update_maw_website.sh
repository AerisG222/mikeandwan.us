#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating functions...';
run_psql_script "funcs/blog.add_post.sql";
run_psql_script "funcs/blog.get_blogs.sql";
run_psql_script "funcs/blog.get_latest_posts.sql";
run_psql_script "funcs/blog.get_post.sql";
run_psql_script "funcs/blog.get_posts.sql";


echo "...${DBNAME} updated.";
