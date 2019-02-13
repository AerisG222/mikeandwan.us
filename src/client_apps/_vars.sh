APPS=(
    "photo_stats"
    "photos3d"
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
    "photos"
    "upload"
    "videos"
    "weekend_countdown"
)

NG_LIBS=(
    "maw-auth"
    "maw-common"
)

NG_CLI_VERSION='@angular/cli@latest'


update_ngcli_global() {
    local isdockerbuild=$1

    if [ "${isdockerbuild}" == "y" ]; then
        npm install -g ${NG_CLI_VERSION}
    else
        sudo npm uninstall -g @angular/cli
        sudo npm cache verify
        sudo npm install -g ${NG_CLI_VERSION}
    fi
}


update_ngcli_project() {
    cd "${1}"

    # clean and install new cli
    npm install --save-dev ${NG_CLI_VERSION}

    # add in any other missing libs
    npm install

    ng update @angular/cli \
              @angular/core \
              @angular-devkit/build-angular \
              @ng-bootstrap/ng-bootstrap \
              core-js \
              codelyzer \
              zone.js \
              webpack-bundle-analyzer

    cd ..
}


install_deps() {
    cd "${1}"
    npm install
    cd ..
}


build_lib() {
    cd "${1}"
    npm run package
    cd ..
}


build_app() {
    cd "${1}"
    npm run "${2}"
    cd ..
}


update_ngcli_all_projects() {
    # do libs first as these could be referenced / needed by the npm install step in the app
    for i in "${NG_LIBS[@]}"
    do
        echo "updating tooling for ${i}..."
        update_ngcli_project "${i}"
        build_lib "${i}"
    done

    for i in "${NG_APPS[@]}"
    do
        echo "updating tooling for ${i}..."
        update_ngcli_project "${i}"
    done
}


install_all_deps() {
    for i in "${APPS[@]}"
    do
        echo "installing dependencies for ${i}..."
        install_deps "${i}"
    done

    for i in "${NG_LIBS[@]}"
    do
        echo "installing dependencies for ${i}..."
        install_deps "${i}"
        build_lib "${i}"
    done

    for i in "${NG_APPS[@]}"
    do
        echo "installing dependencies for ${i}..."
        install_deps "${i}"
    done
}


build_all_apps() {
    local buildcmd=$1

    for i in "${APPS[@]}"
    do
        echo "building ${i}..."
        build_app "${i}" "${buildcmd}"
    done

    for i in "${NG_LIBS[@]}"
    do
        echo "building ${i}..."
        build_lib "${i}"
    done

    for i in "${NG_APPS[@]}"
    do
        echo "building ${i}..."
        build_app "${i}" "${buildcmd}"
    done
}
