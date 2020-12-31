MEDIADIR=/srv/www/website_assets
SRCDIR=/home/mmorano/git/mikeandwan.us/src

if [ ! -L "${SRCDIR}/www/wwwroot/images" ]; then
    ln -s "${MEDIADIR}/images" "${SRCDIR}/www/wwwroot/images"
fi
if [ ! -L "${SRCDIR}/www/wwwroot/movies" ]; then
    ln -s "${MEDIADIR}/movies" "${SRCDIR}/www/wwwroot/movies"
fi

echo '** MEDIA LINKED **'
echo

