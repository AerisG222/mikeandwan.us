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
    "videos"
    "webgl_cube"
    "webgl_text"
    "weekend_countdown"
)

DOWORK=$1
PROD=$2

build() {
    cd "${1}"

    if [ "${PROD}" == 'y' ]; then
        npm run prod_build
    else
        npm run dev_build
    fi

    cd ..
}

if [ "${DOWORK}" == '' ]; then
    echo 'Execute build for all projects? [y/n]'
    read DOWORK
fi

if [ "${DOWORK}" == "y" ]; then
    for I in "${APPS[@]}"
    do
        echo "building ${I}..."
        build "${I}"
    done
fi
