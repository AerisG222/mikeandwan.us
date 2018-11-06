MAW_AUTH_VER="0.0.1"
MAW_COMMON_VER="0.0.7"

# maw-auth
find . -maxdepth 2 -type f -name 'package.json' -exec sed -i "s#maw-auth-.*\.tgz#maw-auth-${MAW_AUTH_VER}\.tgz#g" {} +

# maw-common
find . -maxdepth 2 -type f -name 'package.json' -exec sed -i "s#maw-common-.*\.tgz#maw-common-${MAW_COMMON_VER}\.tgz#g" {} +
