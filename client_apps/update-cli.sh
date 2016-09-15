NG_APPS=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "ng_maw" "photos" "time" "videos" "weekend_countdown" )

update_ngcli_global() {
    sudo npm uninstall -g angular-cli
    sudo npm cache clean
    sudo npm install -g angular-cli
}

update_ngcli() {
    cd "${1}"
    rm -rf node_modules dist tmp typings
    npm install --save-dev angular-cli
    cd ..
}

run_init() {
    cd "${1}"
    ng init
    cd ..
}

# 1: update global tooling
echo 'Update global ng cli? [y/n]'
read GLOBAL_UPDATE

if [ "${GLOBAL_UPDATE}" == "y" ]; then
    update_ngcli_global
fi

# 2: update all project tooling
echo 'Update local project cli? [y/n]'
read LOCAL_UPDATE

if [ "${LOCAL_UPDATE}" == "y" ]; then
    for I in "${NG_APPS[@]}"
    do
        echo "updating tooling for ${I}..."
        update_ngcli "${I}"
    done
fi

# 3: now go through each one and run ng init
echo 'Execute ng init per project? [y/n]'
read NG_INIT

if [ "${NG_INIT}" == "y" ]; then
    for I in "${NG_APPS[@]}"
    do
        echo "executing ng init for project ${I}"
        run_init "${I}"
    done
fi
