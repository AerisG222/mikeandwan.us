MEDIADIR=/srv/www/website_assets
SRCDIR=/home/mmorano/git/mikeandwan.us/src

if [ ! -L "${SRCDIR}/mikeandwan.us/wwwroot/images" ]; then
    ln -s "${MEDIADIR}/images" "${SRCDIR}/mikeandwan.us/wwwroot/images"
fi
if [ ! -L "${SRCDIR}/mikeandwan.us/wwwroot/movies" ]; then
    ln -s "${MEDIADIR}/movies" "${SRCDIR}/mikeandwan.us/wwwroot/movies"
fi
