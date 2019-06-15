APPS=(
    "webgl_blender_model"
    "webgl_cube"
    "webgl_shader"
    "webgl_text"
)

NG_APPS=(
    "binary_clock"
    "googlemaps"
    "learning"
    "memory"
    "money_spin"
    "weekend_countdown"
)

NG_CLI_VERSION='@angular/cli@latest'


update_ngcli_global() {
    local isdockerbuild=$1

    if [ "${isdockerbuild}" == "y" ]; then
        npm install -g ${NG_CLI_VERSION}
    else
        sudo npm uninstall -g @angular/cli
        npm cache verify
        sudo npm install -g ${NG_CLI_VERSION}
    fi
}


build_app() {
    cd "${1}"
    npm run "${2}"
    cd ..
}


build_all_apps() {
    local buildcmd=$1

    for i in "${APPS[@]}"
    do
        echo "building ${i}..."
        build_app "${i}" "${buildcmd}"
    done

    for i in "${NG_APPS[@]}"
    do
        echo "building ${i}..."
        build_app "${i}" "${buildcmd}"
    done
}
