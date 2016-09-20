# script to prepare the production build
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
MEDIADIR=/srv/www/website_assets
SRCDIR=/home/mmorano/git/mikeandwan.us
DEBUG=n
WD=$( pwd )

TSAPPS=(
    "photo_stats"
    "webgl_cube"
    "webgl_text"
)

NGAPPS=( 
    "bandwidth"
    "binary_clock"
    "byte_counter"
    "filesize"
    "googlemaps"
    "learning"
    "memory"
    "money_spin"
    "photos"
    "time"
    "videos"
    "weekend_countdown"
)

copy_app() {
    local APP=$1

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -avh --include '*/' --include '*.js' --exclude '*' "${SRCDIR}/client_apps/${APP}/dist/" "${SRCDIR}/dist/wwwroot/js/${APP}"
}

uglify_app() {
    local APP=$1

    echo "${SRCDIR}/dist/wwwroot/js/${APP}"

    for JS in $( find "${SRCDIR}/dist/wwwroot/js/${APP}" -name '*.js' ); do
        local DIRNAME=$( dirname "$JS" )
        local FILENAME=$( basename "$JS" )
        local EXTENSION="${FILENAME##*.}"
        local FILENAME="${FILENAME%.*}"
        local SHA1=$( sha1sum "${JS}" | awk '{print $1}' )

        uglifyjs "${JS}" --mangle --compress --output "${DIRNAME}/${FILENAME}.${SHA1}.${EXTENSION}"

        rm "${JS}"
        rm "${JS}.map"
    done
}

update_script_refs() {
    local APP=$1

    for JS in $( find "${SRCDIR}/dist/wwwroot/js/${APP}" -name '*.js' ); do
        local NEWFILENAME=$( basename "$JS" )
        local EXTENSION="${NEWFILENAME##*.}"
        local ORIGFILENAME="${NEWFILENAME%%.*}"

        local ORIGURL="/js/${APP}/${ORIGFILENAME}.${EXTENSION}"
        local NEWURL="/js/${APP}/${NEWFILENAME}"

        # this is a bit of a hack, but not sure how to best improve this right now...
        #
        # angular apps add 'bundle' to the name, and their hash is between the real
        # filename part and the 'bundle.js' part.  this check injects the .bundle back
        # in if needed
        if [[ ${NEWURL} == *"bundle"* ]]; then
            ORIGURL="/js/${APP}/${ORIGFILENAME}.bundle.${EXTENSION}"
        fi

        if [ ${DEBUG} == 'y' ]; then
            echo ''
            echo "ORIG: ${ORIGURL}"
            echo "NEW: ${NEWURL}"
            echo ''
        fi

        find "${SRCDIR}/dist" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${NEWURL}#g" {} +
    done
}

ensure_dir() {
    local APP=$1

    if [ ! -d "${SRCDIR}/dist/wwwroot/js/${APP}" ]; then
        mkdir "${SRCDIR}/dist/wwwroot/js/${APP}"
    fi
}

publish_apps() {
    # non-angular apps won't have the hash in their filename
    for APP in ${TSAPPS[@]}; do
        ensure_dir "${APP}"
        copy_app "${APP}"
        uglify_app "${APP}"
        update_script_refs "${APP}"
    done

    # angular cli will bake in a hash to the production filenames
    for APP in ${NGAPPS[@]}; do
        ensure_dir "${APP}"
        copy_app "${APP}"
        update_script_refs "${APP}"
    done
}

link_media() {
    local LINKROOT=$1

    if [ ! -L "${LINKROOT}/images" ]; then
        ln -s "${MEDIADIR}/images_new" "${LINKROOT}/images"
    fi
    if [ ! -L "${LINKROOT}/movies" ]; then
        ln -s "${MEDIADIR}/movies" "${LINKROOT}/movies"
    fi
}

unlink_media() {
    local LINKROOT=$1

    if [ -L "${LINKROOT}/images" ]; then
        rm "${LINKROOT}/images"
    fi
    if [ -L "${LINKROOT}/movies" ]; then
        rm "${LINKROOT}/movies"
    fi
}


echo '***************************************'
echo '** STEP 1: build client applications **'
echo '***************************************'
cd "${SRCDIR}/client_apps"
./rebuild-all.sh y y
cd "${WD}"

echo ''
echo '**************************************'
echo '** STEP 2: prepare production build **'
echo '**************************************'
if [ -d "${SRCDIR}/dist" ]; then
    rm -rf "${SRCDIR}/dist"
fi

# remove dev links, otherwise these are copied during publish
unlink_media "${SRCDIR}/mikeandwan.us/wwwroot"

cd "${SRCDIR}/mikeandwan.us"
dotnet publish -f netcoreapp1.0 -o "${SRCDIR}/dist" -c Release

publish_apps

# add the media links for testing
link_media "${SRCDIR}/dist/wwwroot"

echo ''
echo '**************************************************************'
echo '** STEP 3: go to localhost:5000 to test - hit CTL-C to quit **'
echo '**************************************************************'
cd "${SRCDIR}/dist"
( dotnet "mikeandwan.us.dll" ) 

# cleanup
unlink_media "${SRCDIR}/dist/wwwroot"

echo ''
echo '********************'
echo '** STEP 4: deploy **'
echo '********************'
echo 'Would you like to deploy to production? [y/n]'
read dodeploy

if [ "${dodeploy}" == "y" ]; then
    rsync -avh "${SRCDIR}/dist" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

    ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
        echo \"These commands will be run on: \$( uname -n )\"

        if [ -d /srv/www/_staging ]; then
            sudo rm -rf /srv/www/_staging
        fi

        sudo mv dist /srv/www/_staging

        sudo restorecon -R /srv/www/_staging
        
        sudo systemctl stop supervisord.service
        sudo systemctl stop nginx.service
        sudo systemctl stop mikeandwan.us.service

        killall dotnet
        
        if [ -d /srv/www/mikeandwan.us ]; then
            if [ -d /srv/www/mikeandwan.us_old ]; then
                sudo rm -rf /srv/www/mikeandwan.us_old
            fi

            sudo mv /srv/www/mikeandwan.us /srv/www/mikeandwan.us_old
        fi

        sudo mv /srv/www/_staging /srv/www/mikeandwan.us

        sudo systemctl start mikeandwan.us.service
        sudo systemctl start nginx.service
        sudo systemctl start supervisord.service
    "
fi

echo ''
echo '**********'
echo '** DONE **'
echo '**********'
echo ''
