#!/bin/bash

# for prod, we need a slightly different setup for the gateway seeing we need to support
# official certs from Let's Encrypt.  as such, run the gateway as follows:
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-certbot-validation:/var/www/certbot:ro,z \
    -v maw-certbot-certs:/etc/letsencrypt:ro,z \
    -v /srv/www/website_assets:/assets:ro \
    --security-opt label=disable \
       maw-gateway-dev

# certbot container
podman run -it \
 --pod maw-pod \
    -v maw-certbot-validation:/var/www/certbot:rw,z \
    -v maw-certbot-certs:/etc/letsencrypt:rw,z \
    --entrypoint=sh \
       certbot/certbot
       /bin/sh -c 'trap exit TERM; while :; do certbot renew; sleep 12h & wait ${!}; done;'
