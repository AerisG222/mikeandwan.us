###################################################################################################
## script to automate deployment of photos for www.mikeandwan.us.  this script assumes that you
## have configured ssh keys for passwordless authentication.  if that is not configured, please 
## refer to: https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys--2 or expect
## to type in your password
################################################################################################### 
DEBUG=y
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
PATH_LOCAL_SIZE_PHOTOS="~mmorano/git/SizePhotos/src/SizePhotos"
PATH_LOCAL_GLACIER_BACKUP="~/mmorano/git/GlacierBackup/src/GlacierBackup"
PATH_LOCAL_ASSET_SOURCE=
PATH_ASSET_ROOT="/srv/www/website_assets"
CAT_NAME=
YEAR=
PRIVATE_FLAG=

get_value() {
    local prompt=$1
    local secure=$2
    local val=

    while [ "${val}" = "" ]
    do
        if [ "${secure}" = "y" ]; then
            read -r -s -p "${prompt}" val
        else
            read -r -p "${prompt}" val
        fi
    done

    echo "${val}"
}

if [ "${SSH_REMOTE_HOST}" = "" ]; then
    SSH_REMOTE_HOST=$(get_value 'SSH Host: ' 'n')
fi

if [ "${SSH_USERNAME}" = "" ]; then
    SSH_USERNAME=$(get_value 'SSH Username: ' 'n')
fi

if [ "${PATH_LOCAL_ASSET_SOURCE}" = "" ]; then
    PATH_LOCAL_ASSET_SOURCE=$(get_value 'Path to assets: ' 'n')
fi

if [ "${CAT_NAME}" = "" ]; then
    CAT_NAME=$(get_value 'Category name: ' 'n')
fi

if [ "${YEAR}" = "" ]; then
    YEAR=$(get_value 'Category year: ' 'n')
fi

if [ "${PRIVATE_FLAG}" = "" ]; then
    priv=$(get_value 'Is Private [y/n]: ' 'n')

    if [ "${priv}" = 'y' ]; then
        PRIVATE_FLAG="-x"
    fi
fi

# determine sql filename
ASSET_ROOT=`dirname "${PATH_LOCAL_ASSET_SOURCE}"`
DEST_ASSET_ROOT="${PATH_ASSET_ROOT}/images/${YEAR}"
CATEGORY_DIRECTORY_NAME=`basename "${PATH_LOCAL_ASSET_SOURCE}"`
DEST_ASSET_PATH="${DEST_ASSET_ROOT}/${CATEGORY_DIRECTORY_NAME}"
SQL_FILE="${CATEGORY_DIRECTORY_NAME}".sql
PATH_LOCAL_SQL_FILE="${ASSET_ROOT}"/"${SQL_FILE}"
PATH_LOCAL_GLACIER_SQL_FILE="${SQL_FILE}".glacier.sql

if [ "${DEBUG}" = "y" ]; then
    echo 'Command variables: '
    echo ''
    echo "DEBUG: ${DEBUG}"
    echo "SSH_REMOTE_HOST: ${SSH_REMOTE_HOST}"
    echo "SSH_USERNAME: ${SSH_USERNAME}"
    echo "SSH_ROOT_PASSWORD: ${SSH_ROOT_PASSWORD}"
    echo "PATH_LOCAL_SIZE_PHOTOS: ${PATH_LOCAL_SIZE_PHOTOS}"
    echo "PATH_LOCAL_ASSET_SOURCE: ${PATH_LOCAL_ASSET_SOURCE}"
    echo "PATH_ASSET_ROOT: ${PATH_ASSET_ROOT}"
    echo "CAT_NAME: ${CAT_NAME}"
    echo "YEAR: ${YEAR}"
    echo "PATH_LOCAL_SQL_FILE: ${PATH_LOCAL_SQL_FILE}"
    echo "PRIVATE_FLAG: ${PRIVATE_FLAG}"
    echo ''
    echo "ASSET_ROOT: ${ASSET_ROOT}"
    echo "DEST_ASSET_ROOT: ${DEST_ASSET_ROOT}"
    echo "DEST_ASSET_PATH: ${DEST_ASSET_PATH}"
    echo "CATEGORY_DIRECTORY_NAME: ${CATEGORY_DIRECTORY_NAME}"
    echo "SQL_FILE: ${SQL_FILE}"
    echo "PATH_LOCAL_SQL_FILE: ${PATH_LOCAL_SQL_FILE}"
    echo "PATH_LOCAL_GLACIER_SQL_FILE: ${PATH_LOCAL_GLACIER_SQL_FILE}"
    echo ''
    echo '################################################'
    echo 'If this does not look right, hit Ctl-C to cancel'
    echo '################################################'
fi

echo 'processing photos...'
dotnet run -p "${PATH_LOCAL_SIZE_PHOTOS}" SizePhotos -i -c "${CAT_NAME}" -o "${PATH_LOCAL_SQL_FILE}" -p "${PATH_LOCAL_ASSET_SOURCE}" -w "${PATH_LOCAL_ASSET_SOURCE}" -y ${YEAR} ${PRIVATE_FLAG}

# after processing the first time, we typically want to manually review all the photos to make sure
# we keep the good ones and toss the bad / dupes /etc.  This check allows the user to bail if this
# is the first run through
read -r -p "Deploy assets now? [y/n]: " DEPLOY

if [ ! "${DEPLOY}" = "y" ]; then
    echo 'exiting w/o deploying...'
    exit
fi


#################################################
## LOCAL PROCESSING
#################################################
echo 'moving photos for local site...'
mv "${PATH_LOCAL_ASSET_SOURCE}" "${DEST_ASSET_ROOT}"

echo 'applying sql to local database...'
psql -d maw_website -f "${PATH_LOCAL_SQL_FILE}"

echo 'backing up photos to AWS Glacier...'
dotnet run -p "${PATH_LOCAL_GLACIER_BACKUP}" GlacierBackup ...

echo 'applying glacier sql file to local database...'
psql -d maw_website -f "${PATH_LOCAL_GLACIER_SQL_FILE}"


#################################################
## REMOTE PROCESSING
#################################################
echo 'copying files to remote...'
scp -r -c blowfish "${DEST_ASSET_PATH}/xs" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/"${ASSET_ROOT}/xs"
scp -r -c blowfish "${DEST_ASSET_PATH}/sm" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/"${ASSET_ROOT}/sm"
scp -r -c blowfish "${DEST_ASSET_PATH}/md" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/"${ASSET_ROOT}/md"
scp -r -c blowfish "${DEST_ASSET_PATH}/lg" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/"${ASSET_ROOT}/lg"
scp -r -c blowfish "${DEST_ASSET_PATH}/prt" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/"${ASSET_ROOT}/prt"
scp -r -c blowfish "${PATH_LOCAL_SQL_FILE}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~
scp -r -c blowfish "${PATH_LOCAL_GLACIER_SQL_FILE}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~

echo 'deploying...'
ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" <<HERE
    sudo mkdir "${DEST_ASSET_ROOT}"
    sudo mv "~mmorano/${ASSET_ROOT}" "${DEST_ASSET_ROOT}"
    sudo chown -R root:root "${DEST_ASSET_PATH}"
    sudo chmod -R go-w "${DEST_ASSET_PATH}"
    sudo restorecon -R "${DEST_ASSET_PATH}"

    psql -d maw_website -f ${SQL_FILE}
    psql -d maw_website -f ${GLACIER_SQL_FILE}

    rm ${SQL_FILE}
    rm ${GLACIER_SQL_FILE}

HERE


#################################################
## FINAL CLEANUP
#################################################
rm "${PATH_LOCAL_SQL_FILE}"
rm "${PATH_LOCAL_GLACIER_SQL_FILE}"
