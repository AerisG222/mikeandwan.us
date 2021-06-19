#!/bin/bash

# delete prt directories modified 1 year or more ago
find /srv/www/website_assets/images -type d -regex '.*/prt' -ctime +365 -print0 | xargs -0 -n1 rm -rf
