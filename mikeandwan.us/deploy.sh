#!/bin/bash

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
