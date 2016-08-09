apps=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "ng_maw" "photos" "time" "videos" "weekend_countdown" )

update() {
    cd "${1}"
    npm install
    cd ..
}

echo 'Install dependencies for all projects? [y/n]'
read dowork

if [ "${dowork}" == "y" ]; then
    for i in "${apps[@]}"
    do
        echo "updating libs for ${i}..."
        update "${i}"
    done
fi
