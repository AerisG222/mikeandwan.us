SITE=$1
SRC_ROOT="/build/src"
SRC_WWW="${SRC_ROOT}/${SITE}"
DEBUG=n


copy_app() {
    local APP=$1

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -ah --include '*/' --include '*.js' --include '*.css' --exclude '*' "${SRC_ROOT}/client_apps/${APP}/dist/" "${SRC_WWW}/wwwroot/js/${APP}"
}


update_refs() {
    local APP=$1

    for JS in $( find "${SRC_WWW}/wwwroot/js/${APP}" -name '*.js' -o -name '*.css' ); do
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

        if [ ${DEBUG} = 'y' ]; then
            echo ''
            echo "ORIG: ${ORIGURL}"
            echo "NEW: ${NEWURL}"
            echo ''
        fi

        find "${SRC_WWW}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${NEWURL}#g" {} +
    done
}


ensure_dir() {
    local APP=$1

    if [ ! -d "${SRC_WWW}/wwwroot/js/${APP}" ]; then
        mkdir "${SRC_WWW}/wwwroot/js/${APP}"
    fi
}


publish_client_apps() {
    for APP in ${APPS[@]}; do
        ensure_dir "${APP}"
        copy_app "${APP}"
        update_refs "${APP}"
    done
}


minify_css() {
    local FILE=$1
    local ORIG="${SRC_WWW}/wwwroot/css/${FILE}.css"
    local ORIGURL="/css/${FILE}.css"
    local MIN="${SRC_WWW}/wwwroot/css/${FILE}.min.css"

    cleancss -o "${MIN}" "${ORIG}"

    local MD5="$(md5sum ${MIN} |cut -c 1-8)"
    local MD5FILE="${SRC_WWW}/wwwroot/css/${FILE}.min.${MD5}.css"
    local MD5URL="/css/${FILE}.min.${MD5}.css"

    mv "${MIN}" "${MD5FILE}"
    rm "${ORIG}"

    find "${SRC_WWW}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${MD5URL}#g" {} +
}
