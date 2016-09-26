###################################################################################################
## script to bulk copy images to remote server
###################################################################################################
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
PATH_IMAGE_ROOT="/srv/www/website_assets/images_new"
DEST_IMAGE_ROOT="/srv/www/website_assets/images"

echo '***************************************************'
echo '** DELETE LIVE ASSETS BEFORE RUNNING THIS SCRIPT **'
echo '***************************************************'
echo ''

ASSET_ROOT=`basename "${PATH_IMAGE_ROOT}"`

cd "$PATH_IMAGE_ROOT"

rsync -ah --exclude "*/src*" "${PATH_IMAGE_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

echo 'deploying (please provide remote password when prompted)...'
ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
    echo \"These commands will be run on: \$( uname -n )\"

    sudo mv '${ASSET_ROOT}' '${DEST_IMAGE_ROOT}'
    sudo chown -R root:root '${DEST_IMAGE_ROOT}'
    sudo chmod -R go-w '${DEST_IMAGE_ROOT}'
    sudo restorecon -R '${DEST_IMAGE_ROOT}'
"
