#!/bin/bash
ROOT_PATH=/home/mmorano/code/mikeandwan.us/mikeandwan.us
BUILD_PATH=$ROOT_PATH/Maw.Website
PUBLISH_PATH=$ROOT_PATH/production

cd $ROOT_PATH

if [ -d "$BUILD_PATH" ]; then
    rm -rf "$BUILD_PATH"
fi

#if [ -d "$PUBLISH_PATH" ]; then
#    rm -rf "$PUBLISH_PATH"
#fi

npm run gulp -- rebuild-prod

cd $BUILD_PATH

dnu publish . -o "$PUBLISH_PATH" --no-source

echo
echo
echo
echo '******************************************************'
echo 'Completed.'
echo 'Please review prior output to determine build status.'
echo 'To deploy, copy the production directory to the live server and restart this service.'
echo '******************************************************'
