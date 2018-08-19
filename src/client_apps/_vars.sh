APPS=(
    "bandwidth"
    "binary_clock"
    "byte_counter"
    "filesize"
    "googlemaps"
    "learning"
    "memory"
    "money_spin"
    "photo_stats"
    "photos"
    "photos3d"
    "time"
    "upload-client"
    "videos"
    "webgl_cube"
    "webgl_shader"
    "webgl_text"
    "weekend_countdown"
)

NG_CLI_VERSION='@angular/cli@latest'


install_ngcli_global() {
    local isdockerbuild=$1

    if [ "${isdockerbuild}" == "y" ]; then
        npm install -g ${NG_CLI_VERSION}
    else
        sudo npm uninstall -g @angular/cli
        sudo npm cache clean ${NG_CLI_VERSION}
        sudo npm cache verify
        sudo npm install -g ${NG_CLI_VERSION}
    fi
}


install_libs() {
    cd "${1}"

    if [ -e 'package-lock.json' ]
    then
        npm ci
    else
        rm -rf node_modules
        npm install
    fi

    cd ..
}


install_all_libs() {
    for i in "${APPS[@]}"
    do
        echo "installing libs for ${i}..."
        install_libs "${i}"
    done
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
}
