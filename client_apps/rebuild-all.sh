apps=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "ng_maw" "photos" "time" "videos" "weekend_countdown" )

build() {
    cd "${1}"
    ng build && ng lint
    cd ..
}

echo 'Execute build for all projects? [y/n]'
read dowork

if [ "${dowork}" == "y" ]; then
    for i in "${apps[@]}"
    do
        echo "building ${i}..."
        build "${i}"
    done
fi