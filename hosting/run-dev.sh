#!/bin/bash

# postgres
podman run -dt \
  -pod maw-pod \
    -v maw-postgres:/var/lib/postgresql/data:rw,z \
       postgres:12.2

# solr
podman run -dt \
 --pod maw-pod \
    -v maw-solr:/var/solr/data:rw,z \
       maw-solr-dev

# photos
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
       maw-photos-dev

# files
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
       maw-files-dev

# auth
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-auth-dataprotection:/dataprotection:rw,Z \
    --env-file /home/mmorano/git/maw-auth.env \
       maw-auth-dev

# api
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-api-dataprotection:/dataprotection:rw,Z \
    -v maw-uploads:/maw-uploads:rw,z \
    -v /srv/www/website_assets/images:/maw-www/wwwroot/images:ro \
    --security-opt label=disable \
    --env-file /home/mmorano/git/maw-api.env \
       maw-api-dev

# www
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v maw-www-dataprotection:/dataprotection:rw,Z \
    -v /srv/www/website_assets/images:/maw-www/wwwroot/images:ro \
    -v /srv/www/website_assets/movies:/maw-www/wwwroot/movies:ro \
    --security-opt label=disable \
    --env-file /home/mmorano/git/maw-www.env \
       maw-www-dev

# gateway
podman run -dt \
 --pod maw-pod \
    -v maw-certs:/certs:ro,z \
    -v /srv/www/website_assets:/assets:ro \
    --security-opt label=disable \
       maw-gateway-dev


# redirect port 80 and 443 to 8080 and 8443 respectively (temporarily - add --permanent to change)
# https://stackoverflow.com/questions/413807/is-there-a-way-for-non-root-processes-to-bind-to-privileged-ports-on-linux/31795603#31795603
# https://serverfault.com/questions/654102/forwarding-ports-on-centos-7/713849

# the following works for external access
sudo firewall-cmd --add-masquerade
sudo firewall-cmd --add-forward-port=port=80:proto=tcp:toport=8080
sudo firewall-cmd --add-forward-port=port=443:proto=tcp:toport=8443

# the following is for access on localhost
sudo iptables -A OUTPUT -t nat -p tcp --destination 127.0.0.1 --dport 80 -j REDIRECT --to-port 8080
sudo iptables -A OUTPUT -t nat -p tcp --destination 127.0.0.1 --dport 443 -j REDIRECT --to-port 8443
