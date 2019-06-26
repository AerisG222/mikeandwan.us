#!/bin/bash
DBNAME="maw_website";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

echo 'creating functions...';
run_psql_script "funcs/blog.add_post.sql";
run_psql_script "funcs/blog.get_blogs.sql";
run_psql_script "funcs/blog.get_posts.sql";

run_psql_script "funcs/maw.add_login_history.sql";
run_psql_script "funcs/maw.add_role.sql";
run_psql_script "funcs/maw.add_user_to_role.sql";
run_psql_script "funcs/maw.add_user.sql";
run_psql_script "funcs/maw.get_countries.sql";
run_psql_script "funcs/maw.get_role_names.sql";
run_psql_script "funcs/maw.get_roles_for_user.sql";
run_psql_script "funcs/maw.get_roles.sql";
run_psql_script "funcs/maw.get_states.sql";
run_psql_script "funcs/maw.get_user_detail.sql";
run_psql_script "funcs/maw.get_usernames.sql";
run_psql_script "funcs/maw.get_users_in_role.sql";
run_psql_script "funcs/maw.get_users.sql";
run_psql_script "funcs/maw.remove_role.sql";
run_psql_script "funcs/maw.remove_user_from_role.sql";
run_psql_script "funcs/maw.remove_user.sql";
run_psql_script "funcs/maw.save_password.sql";
run_psql_script "funcs/maw.save_security_stamp.sql";
run_psql_script "funcs/maw.update_user.sql";

run_psql_script "funcs/photo.get_categories.sql";
run_psql_script "funcs/photo.get_comments.sql";
run_psql_script "funcs/photo.get_photo_metadata.sql";
run_psql_script "funcs/photo.get_photos.sql";
run_psql_script "funcs/photo.get_random_photos.sql";
run_psql_script "funcs/photo.get_ratings.sql";
run_psql_script "funcs/photo.get_years.sql";
run_psql_script "funcs/photo.save_comment.sql";
run_psql_script "funcs/photo.save_rating.sql";

run_psql_script "funcs/video.get_categories.sql";
run_psql_script "funcs/video.get_comments.sql";
run_psql_script "funcs/video.get_ratings.sql";
run_psql_script "funcs/video.get_videos.sql";
run_psql_script "funcs/video.get_years.sql";
run_psql_script "funcs/video.save_comment.sql";
run_psql_script "funcs/video.save_rating.sql";

echo "...${DBNAME} updated.";
