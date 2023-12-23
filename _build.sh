#!/bin/bash

# script to be run WITHIN the build container - do not run on its own
WORKDIR="/src"
SRC_ROOT="${WORKDIR}/src"
DIST_ROOT="${WORKDIR}/dist"

# load js apps array
source ./_client_app_vars.sh


copy_client_app() {
    local APP=$1
    local SITE_ROOT=$2
    local ISNGAPP=$3
    local DEST="${SITE_ROOT}/wwwroot/js/${APP}"

    if [ ${ISNGAPP} == 'y' ]; then
        local APP_DIST_DIR="${SRC_ROOT}/client_apps_ng/dist/${APP}/"
    else
        local APP_DIST_DIR="${SRC_ROOT}/client_apps/${APP}/dist/"
    fi

    echo "Copying client app from ${APP_DIST_DIR} to ${DEST}"

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -ahm \
          --include '*/'     \
          --include '*.js'   \
          --include '*.css'  \
          --include '*.jpg'  \
          --include '*.png'  \
          --include '*.json' \
          --include '*.gltf' \
          --include '*.bin'  \
          --include '*.mp3'  \
          --include '*.ogg'  \
          --exclude '*'      \
          "${APP_DIST_DIR}"  \
          "${DEST}"
}

update_refs() {
    local APP=$1
    local SITE_ROOT=$2

    for JS in $( find "${SITE_ROOT}/wwwroot/js/${APP}" -name '*.js' -o -name '*.css' ); do
        local NEWFILENAME=$( basename "$JS" )
        local EXTENSION="${NEWFILENAME##*.}"
        local ORIGFILENAME="${NEWFILENAME%%.*}"

        local ORIGURL="/js/${APP}/${ORIGFILENAME}.${EXTENSION}"
        local NEWURL="/js/${APP}/${NEWFILENAME}"

        # angular/webpack apps add 'bundle' to the name, and their hash is between the real
        # filename part and the 'bundle.js' part.  this check injects the .bundle back
        # in if needed
        if [[ ${NEWURL} = *"bundle"* ]]; then
            ORIGURL="/js/${APP}/${ORIGFILENAME}.bundle.${EXTENSION}"
        fi

        find "${SITE_ROOT}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${NEWURL}#g" {} +
    done
}

ensure_dir() {
    local APP=$1
    local SITE_ROOT=$2

    if [ ! -d "${SITE_ROOT}/wwwroot/js/${APP}" ]; then
        mkdir "${SITE_ROOT}/wwwroot/js/${APP}"
    fi
}

publish_client_apps() {
    SITE_ROOT=$1

    for APP in ${APPS[@]}; do
        ensure_dir "${APP}" "${SITE_ROOT}"
        copy_client_app "${APP}" "${SITE_ROOT}" n
        update_refs "${APP}" "${SITE_ROOT}"
    done

    for APP in ${NG_APPS[@]}; do
        ensure_dir "${APP}" "${SITE_ROOT}"
        copy_client_app "${APP}" "${SITE_ROOT}" y
        update_refs "${APP}" "${SITE_ROOT}"
    done
}

minify_assets() {
    local SITE=$1

    minify_css "${SITE}"
    minify_js "${SITE}"
}

minify_js() {
    local SITE=$1
    local SITE_ROOT="${SRC_ROOT}/${SITE}"

    for FILE in $( find "${SITE_ROOT}/wwwroot/js" -maxdepth 1 -name '*.js' )
    do
        # kill path from file
        FILE="${FILE##*/}"

        # kill extension from file
        FILE="${FILE%.js}"

        minify_js_file "${SITE_ROOT}" "${FILE}"
    done
}

minify_js_file() {
    local SITE_ROOT=$1
    local FILE=$2

    local ORIG="${SITE_ROOT}/wwwroot/js/${FILE}.js"
    local ORIGURL="/js/${FILE}.js"
    local MIN="${SITE_ROOT}/wwwroot/js/${FILE}.min.js"

    echo "Minifying JS file: ${ORIG}"

    uglifyjs -o "${MIN}" "${ORIG}"

    local MD5="$(md5sum ${MIN} |cut -c 1-8)"
    local MD5FILE="${SITE_ROOT}/wwwroot/js/${FILE}.min.${MD5}.js"
    local MD5URL="/js/${FILE}.min.${MD5}.js"

    mv "${MIN}" "${MD5FILE}"
    rm "${ORIG}"

    find "${SITE_ROOT}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${MD5URL}#g" {} +
}

minify_css() {
    local SITE=$1
    local SITE_ROOT="${SRC_ROOT}/${SITE}"

    for FILE in $( find "${SITE_ROOT}/wwwroot/css" -name '*.css' )
    do
        # kill path from file
        FILE="${FILE##*/}"

        # kill extension from file
        FILE="${FILE%.css}"

        minify_css_file "${SITE_ROOT}" "${FILE}"
    done
}

minify_css_file() {
    local SITE_ROOT=$1
    local FILE=$2
    local ORIG="${SITE_ROOT}/wwwroot/css/${FILE}.css"
    local ORIGURL="/css/${FILE}.css"
    local MIN="${SITE_ROOT}/wwwroot/css/${FILE}.min.css"

    echo "Minifying CSS file: ${ORIG}"

    cleancss -o "${MIN}" "${ORIG}"

    local MD5="$(md5sum ${MIN} |cut -c 1-8)"
    local MD5FILE="${SITE_ROOT}/wwwroot/css/${FILE}.min.${MD5}.css"
    local MD5URL="/css/${FILE}.min.${MD5}.css"

    mv "${MIN}" "${MD5FILE}"
    rm "${ORIG}"

    find "${SITE_ROOT}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${MD5URL}#g" {} +
}

build_assets() {
    local SITE=$1

    pushd "${SRC_ROOT}/${SITE}"

    echo "Building site assets in ${PWD}"

    # ensure clean builds
    rm -rf node_modules

    pnpm i
    pnpm run sass:lint
    pnpm run sass:site
    pnpm run sass:games

    popd
}

build_site() {
    local SITE=$1

    pushd "${SRC_ROOT}/${SITE}"

    echo "Running dotnet publish from ${PWD} and outputing to ${DIST_ROOT}"

    dotnet publish -o "${DIST_ROOT}" -c release -r linux-musl-x64 --self-contained false
    popd
}

build_api() {
    local SITE="api"

    echo "Starting to build site ${SITE}..."
    build_site "${SITE}"
    echo "Completed building site ${SITE}"
}

build_auth() {
    local SITE="auth"

    echo "Starting to build site ${SITE}..."
    build_assets "${SITE}"
    minify_assets "${SITE}"
    build_site "${SITE}"
    echo "Completed building site ${SITE}"
}

build_www() {
    local SITE="www"

    echo "Starting to build site ${SITE}..."
    build_assets "${SITE}"
    ./build_client_apps.sh y
    minify_assets "${SITE}"
    publish_client_apps "${SRC_ROOT}/${SITE}"
    build_site "${SITE}"
    echo "Completed building site ${SITE}"
}

# remove css / js libs that are replaced with cdns in prod
rm -r "${SRC_ROOT}/auth/wwwroot/js/libs/bootstrap"

rm -r "${SRC_ROOT}/www/wwwroot/js/libs/bootstrap"

if   [ ${1} = 'api' ]; then
    build_api
elif [ ${1} = 'auth' ]; then
    build_auth
elif [ ${1} = 'www' ]; then
    build_www
else
    echo 'Invalid site name - expecting "api", "auth", or "www"'
fi

# cleanup image
rm -rf "${SRC_ROOT}"
