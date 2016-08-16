# script to prepare the production build
SSH_REMOTE_HOST=tifa
SSH_USERNAME=mmorano


echo '** STEP 1: build client applications'
cd ../client_apps
./rebuild-all.sh

echo '** STEP 2: prepare production build'
cd ../

if [ -d "dist" ]; then
    rm -rf "dist"
fi

cd "mikeandwan.us"
dotnet publish -f netcoreapp1.0 -o ../dist -c Release

# hmm, the included assets are not included, so manually copy these for now
cp -PR Views ../dist
cp -PR wwwroot ../dist
cp project.json ../dist
cp config.json ../dist

echo '** STEP 3: go to localhost:5000 to test'
cd ../dist
( dotnet mikeandwan.us.dll )

echo ''
echo '********************'
echo '** STEP 4: deploy **'
echo '********************'
echo 'Would you like to deploy? [y/n]'
read dodeploy

if [ "${dodeploy}" == "y" ]; then
    ssh -t "${SSH_USERNAME}"@"${SSH_REMOTE_HOST}" "
        mv /home/mmorano/code/mikeandwan.us_mvc/_staging /srv/www/_staging

        ln -s /srv/www/website_assets/photos/ /srv/www/_staging/images
        ln -s /srv/www/website_assets/flash/ /srv/www/_staging/swf
        ln -s /srv/www/website_assets/videos /srv/www/_staging/videos

        rm -rf /srv/www/mikeandwan.us_old

        systemctl stop nginx.service
        systemctl stop mikeandwan.us.service

        mv /srv/www/mikeandwan.us /srv/www/mikeandwan.us_old
        mv /srv/www/_staging /srv/www/mikeandwan.us

        systemctl start mikeandwan.us.service
        systemctl start nginx.service
    "
fi

echo '** DONE **'
