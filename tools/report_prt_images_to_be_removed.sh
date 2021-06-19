#!/bin/bash

# find 'prt' directories modfied 1 year or more ago
find /srv/www/website_assets/images -type d -regex '.*/prt' -ctime +365 -print |sort

# print size of files to be removed
find /srv/www/website_assets/images/ -regex '.*/prt/.*' -ctime +365 -print0 | du --files0-from=- -ch | tail -1
