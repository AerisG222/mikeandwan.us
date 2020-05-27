#!/bin/bash
PROD=$1

source ./_client_app_vars.sh


clean_app() {
    export NG_CLI_ANALYTICS=off

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
        clean_app
    fi

    echo "building ${app}..."
    npm run "${buildcmd}"

    popd
}

build_ng_apps() {
    local buildcmd=$1
    local clean=$2

    if [ "${clean}" == 'y' ]; then
        clean_app
    fi

    ng analytics off

    echo "building ${app}..."
    npm run "${buildcmd}"
}

build_all_apps() {
    local prod=$1

    pushd src/client_apps

    for i in "${APPS[@]}"
    do
        if [ "${prod}" == 'y' ]; then
            build_app "${i}" prod_build y
        else
            build_app "${i}" dev_build n
        fi
    done

    popd

    pushd src/client_apps_ng

    if [ "${prod}" == 'y' ]; then
        build_ng_apps build-prod y
    else
        build_ng_apps build-dev n
    fi

    popd
}

build_all_apps "${PROD}"
