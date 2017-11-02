NG_APPS=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "ng_maw" "photos" "time" "videos" "weekend_countdown" )
CLI_VERSION='@angular/cli@latest'

update_ngcli_global() {
    # global update
    sudo npm uninstall -g @angular/cli
    sudo npm cache clean
    sudo npm cache verify
    sudo npm install -g ${CLI_VERSION}
}

update_ngcli() {
    cd "${1}"

    # clean and install new cli
    rm -rf node_modules dist
    npm install --save-dev ${CLI_VERSION}
    npm install

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
