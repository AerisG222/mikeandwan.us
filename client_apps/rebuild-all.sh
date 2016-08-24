apps=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "photos" "time" "videos" "weekend_countdown" )
webgls=( "webgl_cube" "webgl_text" )

DOWORK=$1

build() {
    cd "${1}"
    ng build && ng lint
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
    for i in "${apps[@]}"
    do
        echo "building ${i}..."
        build "${i}"
    done

    for i in "${webgls[@]}"
    do
        echo "building ${i}..."
        buildgl "${i}"
    done
fi
