source src/client_apps/_vars.sh


copy_app() {
    local APP=$1

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -ah --include '*/' --include '*.js' --include '*.css' --exclude '*' "${SRC_ROOT}/client_apps/${APP}/dist/" "${BUILD_WWW}/wwwroot/js/${APP}"
}


