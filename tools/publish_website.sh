# script to prepare the production build
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
MEDIA_ROOT=/srv/www/website_assets
PROJECT_ROOT=/home/mmorano/git/mikeandwan.us
SRC_ROOT="${PROJECT_ROOT}/src"
SRC_WWW="${SRC_ROOT}/mikeandwan.us"
BUILD_ROOT="${PROJECT_ROOT}/build"
BUILD_WWW="${BUILD_ROOT}/mikeandwan.us"
DIST_ROOT="${PROJECT_ROOT}/dist"
DEBUG=n
WD=$( pwd )

JSAPPS=( 
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

copy_app() {
    local APP=$1

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -ah --include '*/' --include '*.js' --include '*.css' --exclude '*' "${SRC_ROOT}/client_apps/${APP}/dist/" "${BUILD_WWW}/wwwroot/js/${APP}"
}

update_refs() {
    local APP=$1

    for JS in $( find "${BUILD_WWW}/wwwroot/js/${APP}" -name '*.js' -o -name '*.css' ); do
        local NEWFILENAME=$( basename "$JS" )
        local EXTENSION="${NEWFILENAME##*.}"
        local ORIGFILENAME="${NEWFILENAME%%.*}"

        local ORIGURL="/js/${APP}/${ORIGFILENAME}.${EXTENSION}"
        local NEWURL="/js/${APP}/${NEWFILENAME}"

        # angular/webpack apps add 'bundle' to the name, and their hash is between the real
        # filename part and the 'bundle.js' part.  this check injects the .bundle back
        # in if needed
        if [[ ${NEWURL} = *"bundle"* ]]; then
            ORIGURL="/js/${APP}/${ORIGFILENAME}.bundle.${EXTENSION}"
        fi

        if [ ${DEBUG} = 'y' ]; then
            echo ''
            echo "ORIG: ${ORIGURL}"
            echo "NEW: ${NEWURL}"
            echo ''
        fi

        find "${BUILD_WWW}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${NEWURL}#g" {} +
    done
}

ensure_dir() {
    local APP=$1

    if [ ! -d "${BUILD_WWW}/wwwroot/js/${APP}" ]; then
        mkdir "${BUILD_WWW}/wwwroot/js/${APP}"
    fi
}

publish_client_apps() {
    for APP in ${JSAPPS[@]}; do
        ensure_dir "${APP}"
        copy_app "${APP}"
        update_refs "${APP}"
    done
}

link_media() {
    local LINKROOT=$1

    if [ ! -L "${LINKROOT}/images" ]; then
        ln -s "${MEDIA_ROOT}/images" "${LINKROOT}/images"
    fi
    if [ ! -L "${LINKROOT}/movies" ]; then
        ln -s "${MEDIA_ROOT}/movies" "${LINKROOT}/movies"
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
DO_BUILD_CLIENT=n
read -e -r -p "Build Client Apps? [y/N]" DO_BUILD_CLIENT
if [ "${DO_BUILD_CLIENT}" = "y" ]; then
    cd "${SRC_ROOT}/client_apps"
    ./rebuild-all.sh y y
    cd "${WD}"
fi

echo ''
echo '**************************************'
echo '** STEP 2: prepare production build **'
echo '**************************************'
if [ -d "${DIST_ROOT}" ]; then
    rm -rf "${DIST_ROOT}"
fi

if [ -d "${BUILD_ROOT}" ]; then
    rm -rf "${BUILD_ROOT}"
fi

mkdir "${BUILD_ROOT}"

# remove dev links, otherwise these are copied during publish
unlink_media "${SRC_WWW}/wwwroot"

# copy the src dirs for building 
# (so we can update views w/ js references to cache bust js filenames)
cp -r "${SRC_ROOT}/Maw.Data" "${BUILD_ROOT}/Maw.Data"
cp -r "${SRC_ROOT}/Maw.Data.EntityFramework" "${BUILD_ROOT}/Maw.Data.EntityFramework"
cp -r "${SRC_ROOT}/Maw.Domain" "${BUILD_ROOT}/Maw.Domain"
cp -r "${SRC_ROOT}/Maw.TagHelpers" "${BUILD_ROOT}/Maw.TagHelpers"
cp -r "${SRC_WWW}" "${BUILD_WWW}"

cd "${BUILD_WWW}"

publish_client_apps

dotnet publish -f netcoreapp1.1 -o "${DIST_ROOT}" -c Release
rm -rf "${BUILD_ROOT}"

# add the media links for testing
link_media "${DIST_ROOT}/wwwroot"

echo ''
echo '**************************************************************'
echo '** STEP 3: go to localhost:5000 to test - hit CTL-C to quit **'
echo '**************************************************************'
cd "${DIST_ROOT}"
( ASPNETCORE_ENVIRONMENT=production dotnet "mikeandwan.us.dll" ) 

# cleanup
unlink_media "${DIST_ROOT}/wwwroot"

echo ''
echo '********************'
echo '** STEP 4: deploy **'
echo '********************'
echo 'Would you like to deploy to production? [y/n]'
read DO_DEPLOY

if [ "${DO_DEPLOY}" = "y" ]; then
    rsync -ah "${DIST_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

    ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
        echo \"These commands will be run on: \$( uname -n )\"

        if [ -d /srv/www/_staging ]; then
            sudo rm -rf /srv/www/_staging
        fi

        sudo mv dist /srv/www/_staging

        sudo chown -R root:root /srv/www/_staging
        sudo restorecon -R /srv/www/_staging
        
        sudo systemctl stop nginx.service
        sudo supervisorctl stop mikeandwan.us
        
        if [ -d /srv/www/mikeandwan.us ]; then
            if [ -d /srv/www/mikeandwan.us_old ]; then
                sudo rm -rf /srv/www/mikeandwan.us_old
            fi

            sudo mv /srv/www/mikeandwan.us /srv/www/mikeandwan.us_old
        fi

        sudo mv /srv/www/_staging /srv/www/mikeandwan.us

        sudo supervisorctl start mikeandwan.us
        sudo systemctl start nginx.service
    "
fi

echo ''
echo '**********'
echo '** DONE **'
echo '**********'
echo ''
