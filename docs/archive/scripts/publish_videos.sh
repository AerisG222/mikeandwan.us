###################################################################################################
## script to automate deployment of videos for www.mikeandwan.us.  this script assumes that you
## have configured ssh keys for passwordless authentication.  if that is not configured, please
## refer to: https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys--2 or expect
## to type in your password
###################################################################################################
DEBUG=y
SSH_REMOTE_HOST=tifa
SSH_USERNAME=svc_www_maw
PATH_ASSET_ROOT="/srv/www/website_assets"
PATH_VIDEO_SOURCE=
CAT_NAME=
YEAR=
ALLOWED_ROLES=

IMG_CONVERT_PHOTOS=docker.io/aerisg222/maw-convert-videos
IMG_GLACIER_BACKUP=docker.io/aerisg222/maw-glacier-backup
IMG_POSTGRES=docker.io/postgres:16-alpine
CPUS_CONVERT_VIDEOS=28
CPUS_GLACIER_BACKUP=12

DEV_POD=dev-maw-pod
DEV_SVC="pod-${DEV_POD}"
DEV_PSQL_ENV_FILE=/home/mmorano/maw_dev/podman-env/maw-postgres.env

PROD_POD=prod-maw-pod
PROD_PSQL_ENV_FILE=/home/svc_www_maw/maw_prod/podman-env/maw-postgres.env

get_value() {
    local prompt=$1
    local secure=$2
    local default=$3
    local val=

    while [ "${val}" = "" ]
    do
        if [ "${secure}" = "y" ]; then
            read -e -r -s -p "${prompt}" val
        else
            read -e -r -p "${prompt}" val
        fi

        if [ "${val}" = "" -a "${default}" != "" ]; then
            val="${default}"
        fi
    done

    echo "${val}"
}

if [ "${PATH_VIDEO_SOURCE}" = "" ]; then
    PATH_VIDEO_SOURCE=$(get_value 'Path to videos: ' 'n')

    # cleanup trailing slash if needed
    PATH_VIDEO_SOURCE="${PATH_VIDEO_SOURCE%/}"

    # now add it back as it is required
    PATH_VIDEO_SOURCE="${PATH_VIDEO_SOURCE}/"
fi

if [ "${CAT_NAME}" = "" ]; then
    CAT_NAME=$(get_value 'Category name: ' 'n')
fi

if [ "${YEAR}" = "" ]; then
    YEAR=$(get_value 'Category year: ' 'n')
fi

roles=($(get_value 'Allowed roles (space delimited list) [friend admin]: ' 'n' 'friend admin'))

for i in "${roles[@]}"; do
    ALLOWED_ROLES="${ALLOWED_ROLES} -r ${i} "
done

# determine sql filename
ASSET_ROOT=`dirname "${PATH_VIDEO_SOURCE}"`
CATEGORY_DIRECTORY_NAME=`basename "${PATH_VIDEO_SOURCE}"`
DEST_MOVIES_ROOT="${PATH_ASSET_ROOT}/movies"
DEST_MOVIES_YEAR_ROOT="${DEST_MOVIES_ROOT}/${YEAR}"
DEST_MOVIES_CATEGORY_ROOT="${DEST_MOVIES_YEAR_ROOT}/${CATEGORY_DIRECTORY_NAME}"
SQL_FILE="${CATEGORY_DIRECTORY_NAME}.sql"
PATH_LOCAL_SQL_FILE="${ASSET_ROOT}/${SQL_FILE}"
GLACIER_SQL_FILE="${SQL_FILE}.glacier.sql"
PATH_LOCAL_GLACIER_SQL_FILE="${ASSET_ROOT}/${GLACIER_SQL_FILE}"

if [ -d "${DEST_MOVIES_CATEGORY_ROOT}" ]; then
    echo ''
    echo '****'
    echo '  This directory already exists in the destination.'
    echo '  If you are trying to add movies to a previously published directory, you must do this manually.'
    echo '  Otherwise, please give the directory a unique name and then try again.'
    echo '****'

    exit
fi

if [ "${DEBUG}" = "y" ]; then
    echo ''
    echo 'VARIABLES: '
    echo "                      DEBUG: ${DEBUG}"
    echo "            SSH_REMOTE_HOST: ${SSH_REMOTE_HOST}"
    echo "               SSH_USERNAME: ${SSH_USERNAME}"
    echo "          PATH_VIDEO_SOURCE: ${PATH_VIDEO_SOURCE}"
    echo "            PATH_ASSET_ROOT: ${PATH_ASSET_ROOT}"
    echo "                   CAT_NAME: ${CAT_NAME}"
    echo "                       YEAR: ${YEAR}"
    echo "              ALLOWED_ROLES: ${ALLOWED_ROLES}"
    echo ''
    echo "                 ASSET_ROOT: ${ASSET_ROOT}"
    echo "    CATEGORY_DIRECTORY_NAME: ${CATEGORY_DIRECTORY_NAME}"
    echo "           DEST_MOVIES_ROOT: ${DEST_MOVIES_ROOT}"
    echo "      DEST_MOVIES_YEAR_ROOT: ${DEST_MOVIES_YEAR_ROOT}"
    echo "  DEST_MOVIES_CATEGORY_ROOT: ${DEST_MOVIES_CATEGORY_ROOT}"
    echo "                   SQL_FILE: ${SQL_FILE}"
    echo "        PATH_LOCAL_SQL_FILE: ${PATH_LOCAL_SQL_FILE}"
    echo "           GLACIER_SQL_FILE: ${GLACIER_SQL_FILE}"
    echo "PATH_LOCAL_GLACIER_SQL_FILE: ${PATH_LOCAL_GLACIER_SQL_FILE}"
    echo ''
    echo '################################################'
    echo 'If this does not look right, hit Ctl-C to cancel'
    echo '################################################'
    echo ''

    read -r -p "Continue? [y/n]: " CONTINUE

    if [ ! "${CONTINUE}" = "y" ]; then
        echo ''
        echo 'Exiting...'

        exit
    fi
fi

if [ -d "${PATH_VIDEO_SOURCE}/raw" ]; then
    # move previously processed source files back to main photo directory
    mv "${PATH_VIDEO_SOURCE}"/raw/* "${PATH_VIDEO_SOURCE}"

    # remove generated directories
    rm -rf "${PATH_VIDEO_SOURCE}/raw"
    rm -rf "${PATH_VIDEO_SOURCE}/full"
    rm -rf "${PATH_VIDEO_SOURCE}/scaled"
    rm -rf "${PATH_VIDEO_SOURCE}/thumbnails"
    rm -rf "${PATH_VIDEO_SOURCE}/thumb_sq"
fi

echo '* processing videos...'

podman run -it --rm \
    --pull newer \
    --memory 0 \
    --cpus "${CPUS_CONVERT_VIDEOS}" \
    --volume "${ASSET_ROOT}:/videodir:rw" \
    --volume "${ASSET_ROOT}:/output:rw" \
    --security-opt label=disable \
    "${IMG_CONVERT_PHOTOS}" \
        -c "${CAT_NAME}" \
        -o "/output/${SQL_FILE}" \
        -v "/videodir/${CATEGORY_DIRECTORY_NAME}" \
        -w "movies" \
        -y ${YEAR} \
        ${ALLOWED_ROLES}

stty sane

# after processing the first time, we typically want to manually review all the photos to make sure
# we keep the good ones and toss the bad / dupes /etc.  This check allows the user to bail if this
# is the first run through
read -r -p "Deploy assets now? [y/n]: " DEPLOY

if [ ! "${DEPLOY}" = "y" ]; then
    echo ''
    echo '****'
    echo '  Exiting w/o deploying...'
    echo '****'

    exit
fi


#################################################
## LOCAL PROCESSING
#################################################
if [ ! -d "${DEST_MOVIES_YEAR_ROOT}" ]; then
    mkdir "${DEST_MOVIES_YEAR_ROOT}"
fi

echo '* make sure dev podman pod is running...'
systemctl --user start "${DEV_SVC}"
sleep 3s

echo '* moving videos for local site...'
mv "${PATH_VIDEO_SOURCE}" "${DEST_MOVIES_YEAR_ROOT}"

echo '* applying sql to local database...'
podman run -it --rm \
    --pull newer \
    --pod "${DEV_POD}" \
    --env-file "${DEV_PSQL_ENV_FILE}" \
    --volume "${ASSET_ROOT}:/output:ro" \
    --security-opt label=disable \
    "${IMG_POSTGRES}" \
        psql \
            -h localhost \
            -U postgres \
            -d maw_website \
            -f "/output/${SQL_FILE}"

echo '* backing up videos to AWS Glacier...'
podman run -it --rm \
    --pull newer \
    --cpus "${CPUS_GLACIER_BACKUP}" \
    --volume "/home/mmorano/.aws:/creds:ro" \
    --volume "${DEST_MOVIES_ROOT}:/videodir:ro" \
    --volume "${ASSET_ROOT}:/output:rw" \
    --security-opt label=disable \
    "${IMG_GLACIER_BACKUP}" \
        glacier_backup \
        us-east-1 \
        videos \
        assets \
        "/videodir/${YEAR}/${CATEGORY_DIRECTORY_NAME}/" \
        "/videodir/" \
        videosql \
        "/output/${GLACIER_SQL_FILE}" \
        /creds/credentials

echo '* applying glacier sql file to local database...'
podman run -it --rm \
    --pull newer \
    --pod "${DEV_POD}" \
    --env-file "${DEV_PSQL_ENV_FILE}" \
    --volume "${ASSET_ROOT}:/output:ro" \
    --security-opt label=disable \
    "${IMG_POSTGRES}" \
        psql \
            -h localhost \
            -U postgres \
            -d maw_website \
            -f "/output/${GLACIER_SQL_FILE}"

#################################################
## REMOTE PROCESSING
#################################################
echo '* copying files to remote...'
rsync -ah --exclude "*/raw*" "${DEST_MOVIES_CATEGORY_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/
scp "${PATH_LOCAL_SQL_FILE}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~
scp "${PATH_LOCAL_GLACIER_SQL_FILE}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~

echo '* deploying (please provide remote password when prompted)...'
ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
    echo \"These commands will be run on: \$( uname -n )\"

    if [ ! -d '${DEST_MOVIES_YEAR_ROOT}' ]; then
        sudo mkdir '${DEST_MOVIES_YEAR_ROOT}'
    fi

    sudo mv '${CATEGORY_DIRECTORY_NAME}' '${DEST_MOVIES_YEAR_ROOT}'
    sudo chown -R root:root '${DEST_MOVIES_CATEGORY_ROOT}'
    sudo chmod -R go-w '${DEST_MOVIES_CATEGORY_ROOT}'
    sudo restorecon -R '${DEST_MOVIES_CATEGORY_ROOT}'

    podman run -it --rm \
        --pod ${PROD_POD} \
        --env-file '${PROD_PSQL_ENV_FILE}' \
        --volume .:/sql:ro \
        --security-opt label=disable \
        ${IMG_POSTGRES} \
            psql \
                -h localhost \
                -U postgres \
                -d maw_website \
                -f '/sql/${SQL_FILE}'

    podman run -it --rm \
        --pod ${PROD_POD} \
        --env-file '${PROD_PSQL_ENV_FILE}' \
        --volume .:/sql:ro \
        --security-opt label=disable \
        ${IMG_POSTGRES} \
            psql \
                -h localhost \
                -U postgres \
                -d maw_website \
                -f '/sql/${GLACIER_SQL_FILE}'

    rm '${SQL_FILE}'
    rm '${GLACIER_SQL_FILE}'
"

#################################################
## FINAL CLEANUP
#################################################
read -r -p "Delete SQL Files? [y/n]: " CONTINUE

if [ "${CONTINUE}" = "y" ]; then
    rm "${PATH_LOCAL_SQL_FILE}"
    rm "${PATH_LOCAL_GLACIER_SQL_FILE}"
fi

echo ''
echo '****'
echo '  Completed. Good bye.'
echo '****'
