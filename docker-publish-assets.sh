source src/client_apps/_vars.sh
source _vars.sh

# remove css / js libs that are replaced with cdns in prod
rm -r "${SRC_WWW}/wwwroot/css/libs"
rm -r "${SRC_WWW}/wwwroot/js/libs/bootstrap"
rm -r "${SRC_WWW}/wwwroot/js/libs/highlight"
rm -r "${SRC_WWW}/wwwroot/js/libs/jquery"
rm -r "${SRC_WWW}/wwwroot/js/libs/reveal"

minify_css 'site'
minify_css 'games'

publish_client_apps
