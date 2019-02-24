###################################################################################################
## script to bulk copy new thumbnails to production
###################################################################################################
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano
PATH_IMAGE_ROOT="/srv/www/website_assets/images"
PATH_MOVIE_ROOT="/srv/www/website_assets/movies"

DEST_ROOT="/srv/www/website_assets/"
DEST_IMAGE_ROOT="${DEST_ROOT}images"
DEST_MOVIE_ROOT="${DEST_ROOT}movies"

IMAGE_ASSET_ROOT=`basename "${PATH_IMAGE_ROOT}"`
MOVIE_ASSET_ROOT=`basename "${PATH_MOVIE_ROOT}"`

# copy new size xs_sq for photos
rsync -ah \
      --prune-empty-dirs \
      --exclude "*/xs/*" \
      --exclude "*/sm/*" \
      --exclude "*/md/*" \
      --exclude "*/lg/*" \
      --exclude "*/prt/*" \
      --exclude "*/src/*" \
      "${PATH_IMAGE_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

# copy updated movie thumbnails (jpg) and new size thumb_sq
rsync -ah \
      --prune-empty-dirs \
      --exclude "*/raw/*" \
      --exclude "*/full/*" \
      --exclude "*/scaled/*" \
      "${PATH_MOVIE_ROOT}" "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}":~/

echo 'deploying (please provide remote password when prompted)...'
ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
    echo \"These commands will be run on: \$( uname -n )\"

    sudo cp -Rn '${IMAGE_ASSET_ROOT}' '${DEST_ROOT}'
    sudo chown -R root:root '${DEST_IMAGE_ROOT}'
    sudo chmod -R go-w '${DEST_IMAGE_ROOT}'
    sudo restorecon -R '${DEST_IMAGE_ROOT}'
    rm -rf '${IMAGE_ASSET_ROOT}'

    sudo cp -Rn '${MOVIE_ASSET_ROOT}' '${DEST_ROOT}'
    sudo chown -R root:root '${DEST_MOVIE_ROOT}'
    sudo chmod -R go-w '${DEST_MOVIE_ROOT}'
    sudo restorecon -R '${DEST_MOVIE_ROOT}'
    rm -rf '${MOVIE_ASSET_ROOT}'

    sudo find '${DEST_MOVIE_ROOT}' -name '*.png' -delete
"

echo '######################################################################################'
echo 'please make sure to run the sql update, to fix video thumbnails now that they are jpg:'
echo ''
echo 'update video.video set thumb_path = REPLACE(thumb_path, ''.png'', ''.jpg'');'
echo 'update video.category set teaser_image_path = REPLACE(teaser_image_path, ''.png'', ''.jpg'');'
echo ''
echo '######################################################################################'
