#!/bin/bash
DBNAME="maw_website";
MIGRATION_SCRIPT="migration.sql";

function run_psql_script() {
    psql -d "${DBNAME}" -q -f "$1";
}

function pop_mig_script() {
    echo "-- $1" >> "${MIGRATION_SCRIPT}"
    echo '' >> "${MIGRATION_SCRIPT}"
    mysql -u "${MYSQL_USER}" -p"${MYSQL_PASSWORD}" -D "${MYSQL_DB}" --raw --column-names=FALSE < "$1" >> migration.sql;
    echo '' >> "${MIGRATION_SCRIPT}";
}

echo "creating database ${DBNAME}...";
database/maw_website.sh "${DBNAME}";

echo 'creating roles...';
run_psql_script "roles/website.sql";

echo 'creating users...';
run_psql_script "users/svc_www_maw.sql";

echo 'creating schemas...';
run_psql_script "schemas/aws.sql";
run_psql_script "schemas/blog.sql";
run_psql_script "schemas/maw.sql";
run_psql_script "schemas/photo.sql";
run_psql_script "schemas/video.sql";

echo 'creating tables...';
run_psql_script "tables/aws.glacier_vault.sql";

run_psql_script "tables/maw.country.sql";
run_psql_script "tables/maw.login_activity_type.sql";
run_psql_script "tables/maw.login_area.sql";
run_psql_script "tables/maw.role.sql";
run_psql_script "tables/maw.state.sql";
run_psql_script "tables/maw.user.sql";
run_psql_script "tables/maw.user_role.sql";
run_psql_script "tables/maw.login_history.sql";

run_psql_script "tables/blog.blog.sql";
run_psql_script "tables/blog.post.sql";

run_psql_script "tables/photo.active_d_lighting.sql";
run_psql_script "tables/photo.af_area_mode.sql";
run_psql_script "tables/photo.af_point.sql";
run_psql_script "tables/photo.auto_focus.sql";
run_psql_script "tables/photo.colorspace.sql";
run_psql_script "tables/photo.compression.sql";
run_psql_script "tables/photo.contrast.sql";
run_psql_script "tables/photo.exposure_mode.sql";
run_psql_script "tables/photo.exposure_program.sql";
run_psql_script "tables/photo.flash.sql";
run_psql_script "tables/photo.flash_color_filter.sql";
run_psql_script "tables/photo.flash_mode.sql";
run_psql_script "tables/photo.flash_setting.sql";
run_psql_script "tables/photo.flash_type.sql";
run_psql_script "tables/photo.focus_mode.sql";
run_psql_script "tables/photo.gain_control.sql";
run_psql_script "tables/photo.gps_altitude_ref.sql";
run_psql_script "tables/photo.gps_direction_ref.sql";
run_psql_script "tables/photo.gps_latitude_ref.sql";
run_psql_script "tables/photo.gps_longitude_ref.sql";
run_psql_script "tables/photo.gps_measure_mode.sql";
run_psql_script "tables/photo.gps_status.sql";
run_psql_script "tables/photo.high_iso_noise_reduction.sql";
run_psql_script "tables/photo.hue_adjustment.sql";
run_psql_script "tables/photo.lens.sql";
run_psql_script "tables/photo.light_source.sql";
run_psql_script "tables/photo.make.sql";
run_psql_script "tables/photo.metering_mode.sql";
run_psql_script "tables/photo.model.sql";
run_psql_script "tables/photo.noise_reduction.sql";
run_psql_script "tables/photo.orientation.sql";
run_psql_script "tables/photo.picture_control_name.sql";
run_psql_script "tables/photo.raw_conversion_mode.sql";
run_psql_script "tables/photo.saturation.sql";
run_psql_script "tables/photo.scene_capture_type.sql";
run_psql_script "tables/photo.scene_type.sql";
run_psql_script "tables/photo.sensing_method.sql";
run_psql_script "tables/photo.sharpness.sql";
run_psql_script "tables/photo.vibration_reduction.sql";
run_psql_script "tables/photo.vignette_control.sql";
run_psql_script "tables/photo.vr_mode.sql";
run_psql_script "tables/photo.white_balance.sql";
run_psql_script "tables/photo.category.sql";
run_psql_script "tables/photo.photo.sql";
run_psql_script "tables/photo.comment.sql";
run_psql_script "tables/photo.rating.sql";

run_psql_script "tables/video.category.sql";
run_psql_script "tables/video.video.sql";

echo "seeding...";
run_psql_script "seed/photo.auto_focus.sql";
run_psql_script "seed/photo.compression.sql";
run_psql_script "seed/photo.contrast.sql";
run_psql_script "seed/photo.exposure_mode.sql";
run_psql_script "seed/photo.exposure_program.sql";
run_psql_script "seed/photo.flash.sql";
run_psql_script "seed/photo.gain_control.sql";
run_psql_script "seed/photo.gps_altitude_ref.sql";
run_psql_script "seed/photo.gps_direction_ref.sql";
run_psql_script "seed/photo.gps_latitude_ref.sql";
run_psql_script "seed/photo.gps_longitude_ref.sql";
run_psql_script "seed/photo.gps_measure_mode.sql";
run_psql_script "seed/photo.gps_status.sql";
run_psql_script "seed/photo.light_source.sql";
run_psql_script "seed/photo.metering_mode.sql";
run_psql_script "seed/photo.orientation.sql";
run_psql_script "seed/photo.raw_conversion_mode.sql";
run_psql_script "seed/photo.scene_capture_type.sql";
run_psql_script "seed/photo.scene_type.sql";
run_psql_script "seed/photo.sensing_method.sql";
run_psql_script "seed/photo.sharpness.sql";

echo "...${DBNAME} created.";

echo 'would you like to migrate data from mysql? [y/N]';
read MIGRATE;

if [ "${MIGRATE}" == "y" ]; then
    echo 'enter mysql username:';
    read MYSQL_USER;

    echo 'enter mysql password:';
    read -s MYSQL_PASSWORD;

    echo 'enter mysql dbname:';
    read MYSQL_DB;

    echo '' > "${MIGRATION_SCRIPT}";

    echo 'generating seed scripts from mysql...';
    pop_mig_script "migration/maw.country.sql";
    pop_mig_script "migration/maw.login_activity_type.sql";
    pop_mig_script "migration/maw.login_area.sql";
    pop_mig_script "migration/maw.role.sql";
    pop_mig_script "migration/maw.state.sql";
    pop_mig_script "migration/maw.user.sql";
    pop_mig_script "migration/maw.user_role.sql";
    pop_mig_script "migration/maw.login_history.sql";
    pop_mig_script "migration/blog.blog.sql";
    pop_mig_script "migration/blog.post.sql";
    pop_mig_script "migration/photo.category.sql";
    pop_mig_script "migration/photo.photo.sql";
    pop_mig_script "migration/photo.comment.sql";
    pop_mig_script "migration/photo.rating.sql";
    pop_mig_script "migration/video.category.sql";
    pop_mig_script "migration/video.video.sql";

    echo 'applying seed scripts to postgres...';
    run_psql_script "${MIGRATION_SCRIPT}";

    echo 'would you like to keep the generated seed scripts? [y/N]:';
    read KEEP_SCRIPTS;

    echo 'please now apply the updated photo scripts using SizePhotos';

    if [ "${KEEP_SCRIPTS}" != "y" ]; then
        rm -f "${MIGRATION_SCRIPT}";
    fi
fi

echo "If there were errors, you might need to run this script as a user with superuser privileges.";
echo "To do so, as the postgres OS user, run psql, then 'CREATE USER <username> WITH SUPERUSER;'";
