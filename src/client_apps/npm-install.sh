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

update() {
    cd "${1}"
    rm -rf node_modules
    yarn install
    cd ..
}

echo 'Install dependencies for all projects? [y/n]'
read DOWORK

if [ "${DOWORK}" == "y" ]; then
    for i in "${APPS[@]}"
    do
        echo "updating libs for ${i}..."
        update "${i}"
    done
fi
