NG_APPS=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "photos" "time" "videos" "weekend_countdown" )
TS_APPS=( "photo_stats" "webgl_cube" "webgl_text" )

DOWORK=$1
PROD=$2

build() {
    cd "${1}"

    if [ "${PROD}" == '' ]; then
        ng build -prod --environment prod
    else
        ng build && ng lint
    fi

    cd ..
}

buildgl() {
    cd "${1}"
    tsc
    cd ..
}

if [ "${DOWORK}" == '' ]; then
    echo 'Execute build for all projects? [y/n]'
    read DOWORK
fi

if [ "${DOWORK}" == "y" ]; then
    for I in "${NG_APPS[@]}"
    do
        echo "building ${I}..."
        build "${I}"
    done

    for I in "${TS_APPS[@]}"
    do
        echo "building ${I}..."
        buildgl "${I}"
    done
fi
