#!/bin/bash
SOCKETFILE="/var/kestrel/maw.sock"
WWWROOT="/srv/www/mikeandwan.us"

if [ -e "${SOCKETFILE}" ]; then
    rm "${SOCKETFILE}"
fi

cd "${WWWROOT}"
/usr/local/bin/dotnet "${WWWROOT}/mikeandwan.us.dll"
