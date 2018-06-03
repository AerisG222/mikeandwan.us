# script to prepare the production build
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
MEDIA_ROOT=/srv/www/website_assets
PROJECT_ROOT=/home/mmorano/git/mikeandwan.us
SRC_ROOT="${PROJECT_ROOT}/src"
BUILD_ROOT="${PROJECT_ROOT}/build"
DIST_ROOT="${PROJECT_ROOT}/dist"
TOOLS_ROOT="${PROJECT_ROOT}/tools"
DEBUG=n
WD=$( pwd )

SITES=(
    "api"
    "auth"
    "www"
)

# load js apps array
source "${SRC_ROOT}/client_apps/_vars.sh"

copy_app() {
    local APP=$1
    local SITE_ROOT=$2

    # https://silentorbit.com/notes/2013/08/rsync-by-extension/
    rsync -ah --include '*/' --include '*.js' --include '*.css' --exclude '*' "${SRC_ROOT}/client_apps/${APP}/dist/" "${SITE_ROOT}/wwwroot/js/${APP}"
}

update_refs() {
    local APP=$1
    local SITE_ROOT=$2

    for JS in $( find "${SITE_ROOT}/wwwroot/js/${APP}" -name '*.js' -o -name '*.css' ); do
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

        find "${SITE_ROOT}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${NEWURL}#g" {} +
    done
}

ensure_dir() {
    local APP=$1
    local SITE_ROOT=$2

    if [ ! -d "${SITE_ROOT}/wwwroot/js/${APP}" ]; then
        mkdir "${SITE_ROOT}/wwwroot/js/${APP}"
    fi
}

publish_client_apps() {
    SITE_ROOT=$1

    for APP in ${APPS[@]}; do
        ensure_dir "${APP}" "${SITE_ROOT}"
        copy_app "${APP}" "${SITE_ROOT}"
        update_refs "${APP}" "${SITE_ROOT}"
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

minify_css() {
    local SITE=$1
    local SITE_ROOT="${BUILD_ROOT}/${SITE}"

    for FILE in $( find "${SITE_ROOT}/wwwroot/css" -name '*.css' )
    do
        # kill path from file
        FILE="${FILE##*/}"

        # kill extension from file
        FILE="${FILE%.css}"

        minify_css_file "${SITE_ROOT}" "${FILE}"
    done
}

minify_css_file() {
    local SITE_ROOT=$1
    local FILE=$2
    local ORIG="${SITE_ROOT}/wwwroot/css/${FILE}.css"
    local ORIGURL="/css/${FILE}.css"
    local MIN="${SITE_ROOT}/wwwroot/css/${FILE}.min.css"

    cleancss -o "${MIN}" "${ORIG}"

    local MD5="$(md5sum ${MIN} |cut -c 1-8)"
    local MD5FILE="${SITE_ROOT}/wwwroot/css/${FILE}.min.${MD5}.css"
    local MD5URL="/css/${FILE}.min.${MD5}.css"

    mv "${MIN}" "${MD5FILE}"
    rm "${ORIG}"

    find "${SITE_ROOT}" -type f -name "*.cshtml" -exec sed -i "s#${ORIGURL}#${MD5URL}#g" {} +
}

build_sass() {
    local SITE=$1

    cd "${SRC_ROOT}/${SITE}"

    npm ci
    npm run sass
}

echo ''
echo '***************************************'
echo '** STEP 1: build css                 **'
echo '***************************************'
build_sass "auth"
build_sass "www"
cd "${WD}"

echo ''
echo '***************************************'
echo '** STEP 2: build client applications **'
echo '***************************************'
DO_BUILD_CLIENT=n
read -e -r -p "Build Client Apps? [y/N] " DO_BUILD_CLIENT
if [ "${DO_BUILD_CLIENT}" = "y" ]; then
    cd "${SRC_ROOT}/client_apps"
    ./rebuild-all.sh y y
    cd "${WD}"
fi

echo ''
echo '**************************************'
echo '** STEP 3: prepare production build **'
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
cp -r "${SRC_ROOT}/Maw.Domain" "${BUILD_ROOT}/Maw.Domain"
cp -r "${SRC_ROOT}/Maw.Security" "${BUILD_ROOT}/Maw.Security"
cp -r "${SRC_ROOT}/Maw.TagHelpers" "${BUILD_ROOT}/Maw.TagHelpers"
cp -r "${SRC_ROOT}/Mvc.RenderViewToString" "${BUILD_ROOT}/Mvc.RenderViewToString"

for SITE in "${SITES[@]}"
do
    cp -r "${SRC_ROOT}/${SITE}" "${BUILD_ROOT}/${SITE}"
done

# remove css / js libs that are replaced with cdns in prod
rm -r "${BUILD_ROOT}/auth/wwwroot/js/libs/bootstrap"

rm -r "${BUILD_ROOT}/www/wwwroot/css/libs"
rm -r "${BUILD_ROOT}/www/wwwroot/js/libs/bootstrap"
rm -r "${BUILD_ROOT}/www/wwwroot/js/libs/highlight"
rm -r "${BUILD_ROOT}/www/wwwroot/js/libs/jquery"
rm -r "${BUILD_ROOT}/www/wwwroot/js/libs/popper"
rm -r "${BUILD_ROOT}/www/wwwroot/js/libs/reveal"

# minify css
minify_css "auth"
minify_css "www"

# publish client apps to www
cd "${BUILD_ROOT}/www"

publish_client_apps "${BUILD_ROOT}/www"

# create production build
for SITE in "${SITES[@]}"
do
    cd "${BUILD_ROOT}/${SITE}"
    dotnet publish -f netcoreapp2.1 -o "${DIST_ROOT}/${SITE}" -c Release
done

rm -rf "${BUILD_ROOT}"

# add the media links for testing
link_media "${DIST_ROOT}/www/wwwroot"

echo ''
echo '**************************************************************'
echo '** STEP 4: go to https://wwwdev.mikeandwan.us:5021 to test  **'
echo '**************************************************************'
PIDS=()

for SITE in "${SITES[@]}"
do
    cd "${DIST_ROOT}/${SITE}"
    ASPNETCORE_ENVIRONMENT=staging dotnet "maw_${SITE}.dll" > "${TOOLS_ROOT}/${SITE}.log" &
    PIDS[${#PIDS[@]}]=$!
done

read -n1 -r -p "Press any key to continue." xxkeyxx

for PID in "${PIDS[@]}"
do
    kill ${PID}
done

# cleanup
unlink_media "${DIST_ROOT}/www/wwwroot"

echo ''
echo ''
echo '********************'
echo '** STEP 5: deploy **'
echo '********************'
echo 'Would you like to deploy to production? [y/n]'
read DO_DEPLOY

if [ "${DO_DEPLOY}" = "y" ]; then
    rsync -ah "${DIST_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

    ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
        echo \"These commands will be run on: \$( uname -n )\"

        SITES=(
            \"api\"
            \"auth\"
            \"www\"
        )

        if [ -d /srv/www/_staging ]; then
            sudo rm -rf /srv/www/_staging
        fi

        sudo mv dist /srv/www/_staging

        sudo chown -R root:root /srv/www/_staging
        sudo restorecon -R /srv/www/_staging

        sudo systemctl stop nginx.service
        sudo systemctl stop maw_us.service

        for SITE in \"${SITES[@]}\"
        do
            if [ -d \"/srv/www/maw_${SITE}\" ]; then
                if [ -d \"/srv/www/maw_${SITE}.old\" ]; then
                    sudo rm -rf \"/srv/www/maw_${SITE}.old\"
                fi

                sudo mv \"/srv/www/maw_${SITE}\" \"/srv/www/maw_${SITE}.old\"
            fi

            sudo mv \"/srv/www/_staging/${SITE}\" \"/srv/www/maw_${SITE}\"
        done

        sudo systemctl start maw_us.service
        sudo systemctl start nginx.service
    "
fi

echo ''
echo '**********'
echo '** DONE **'
echo '**********'
echo ''
