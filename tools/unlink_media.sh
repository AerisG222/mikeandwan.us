SRCDIR=/home/mmorano/git/mikeandwan.us/src

if [ -L "${SRCDIR}/www/wwwroot/images" ]; then
    rm "${SRCDIR}/www/wwwroot/images"
fi
if [ -L "${SRCDIR}/www/wwwroot/movies" ]; then
    rm "${SRCDIR}/www/wwwroot/movies"
fi

echo '** MEDIA UNLINKED **'
echo

