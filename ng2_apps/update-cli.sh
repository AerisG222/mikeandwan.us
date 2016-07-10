apps=( "bandwidth" "binary_clock" "byte_counter" "filesize" "googlemaps" "learning" "memory" "money_spin" "ng_maw" "photos" "time" "videos" "weekend_countdown" )

update_ngcli_global() {
    sudo npm uninstall -g angular-cli
    sudo npm cache clean
    sudo npm install -g angular-cli@latest
}

update_ngcli() {
    cd "${1}"
    rm -rf node_modules dist tmp
    npm install --save-dev angular-cli@latest
    cd ..
}

run_init() {
    cd "${1}"
    ng init
    cd ..
}

# 1: update global tooling
echo 'Update global ng cli? [y/n]'
read globalupdate

if [ "${globalupdate}" == "y" ]; then
    update_ngcli_global
fi

# 2: update all project tooling
echo 'Update local project cli? [y/n]'
read localupdate

if [ "${localupdate}" == "y" ]; then
    for i in "${apps[@]}"
    do
        echo "updating tooling for ${i}..."
        update_ngcli "${i}"
    done
fi

# 3: now go through each one and run ng init
echo 'Execute ng init per project? [y/n]'
read nginit

if [ "${nginit}" == "y" ]; then
    for i in "${apps[@]}"
    do
        echo "executing ng init for project ${i}"
        run_init "${i}"
    done
fi
