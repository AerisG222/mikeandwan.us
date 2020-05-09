#!/bin/bash

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

    if [ "${isdockerbuild}" == 'y' ]; then
        npm install -g ${NG_CLI_VERSION}
    else
        sudo npm uninstall -g @angular/cli
        npm cache verify
        sudo npm install -g ${NG_CLI_VERSION}
    fi
}

clean_app() {
    rm -rf dist
    rm -rf node_modules
    npm ci
}

build_app() {
    local app=$1
    local buildcmd=$2
    local clean=$3

    pushd "${app}"

    if [ "${clean}" == 'y' ]; then
        echo "cleaning ${app}..."
        clean_app "${app}"
    fi

    echo "building ${app}..."
    npm run "${buildcmd}"

    popd
}

build_all_apps() {
    local buildcmd=$1
    local clean=$2

    for i in "${APPS[@]}"
    do
        build_app "${i}" "${buildcmd}" "${clean}"
    done

    for i in "${NG_APPS[@]}"
    do
        build_app "${i}" "${buildcmd}" "${clean}"
    done
}
