#!/bin/bash

source ./podman-shared.sh

# if we need to create the postgres volume, we will also migrate and prepare existing database
create_volume_postgres() {
    local VOL_NAME=${1}

    create_volume ${VOL_NAME}

    if [ $? -eq 1 ]; then
        return
    fi

    if [ ! -d /var/lib/pgsql/ ]; then
        echo '    - /var/lib/pgsql not found - not configuring postgres volume'
        return
    fi

    echo "    - copying postgres databases to volume"

    # copy posgtres db from current system to volume
    # first - please make sure you know the postgres / admin password.  if you don't log in to the current db and set one with: alter user postgres with password 'NEW_PASSWORD';
    PGSQL_VOL_MOUNT_POINT=$(podman volume inspect "${VOL_NAME}" | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
    PGSQL_VOL_MOUNT_ROOT=$(dirname "${PGSQL_VOL_MOUNT_POINT}")

    sudo cp -a /var/lib/pgsql/data/. "${PGSQL_VOL_MOUNT_POINT}"
    sudo chown -R "${USER}":"${USER}" "${PGSQL_VOL_MOUNT_ROOT}"
    sudo chcon -R unconfined_u:object_r:container_file_t:s0 "${PGSQL_VOL_MOUNT_POINT}"
    podman unshare chown -R 999:999 "${PGSQL_VOL_MOUNT_ROOT}"

    # test postgres
    # podman run -it --rm -v maw-postgres:/var/lib/postgresql/data:rw,z -p 5432:5432 postgres:12.2
}

create_volume_solr() {
    local VOL_NAME=${1}

    create_volume ${VOL_NAME}

    if [ $? -eq 1 ]; then
        return
    fi

    sudo test -d /var/solr/data

    if [ $? -eq 1 ]; then
        echo '    - /var/solr/data not found - not configuring solr volume'
        return
    fi

    echo "    - copying solr config to volume"

    # copy solr config+db from current system to volume, then configure permissions for container
    SOLR_VOL_MOUNT_POINT=$(podman volume inspect "${VOL_NAME}" | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
    SOLR_VOL_MOUNT_ROOT=$(dirname "${SOLR_VOL_MOUNT_POINT}")

    sudo cp -R /var/solr/data "${SOLR_VOL_MOUNT_POINT}"
    sudo chown -R "${USER}":"${USER}" "${SOLR_VOL_MOUNT_ROOT}"
    sudo chcon -R unconfined_u:object_r:container_file_t:s0 "${SOLR_VOL_MOUNT_POINT}"
    podman unshare chown -R 8983:8983 "${SOLR_VOL_MOUNT_ROOT}"

    # test solr
    # podman run -it --rm -v maw-solr:/var/solr/data:rw,z -p 8983:8983 maw-solr-dev
}

create_volume_google_creds() {
    local VOL_NAME=${1}

    create_volume ${VOL_NAME}

    if [ $? -eq 1 ]; then
        return
    fi

    if [ ! -f ~/www_emailer.json ]; then
        echo '    - ~/www_emailer.json not found - not configuring google credentials'
        return
    fi

    echo "    - copying google email credentials to volume"

    # copy google creds to volume
    GOOGLECRED_VOL_MOUNT_POINT=$(podman volume inspect maw-google-creds | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
    cp ~/www_emailer.json "${GOOGLECRED_VOL_MOUNT_POINT}"
}

create_volume_gdrive_creds() {
    local VOL_NAME=${1}

    create_volume ${VOL_NAME}

    if [ $? -eq 1 ]; then
        return
    fi

    if [ ! -f ~/credentials.json ]; then
        echo '    - ~/credentials.json not found - not configuring gdrive credentials'
        return
    fi

    echo "    - copying gdrive credentials to volume"

    # follow instructions [here](https://hub.docker.com/r/d0whc3r/gdrive) to get credentials.json file
    GDRIVE_VOL_MOUNT_POINT=$(podman volume inspect maw-gdrive-creds | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
    GDRIVE_VOL_MOUNT_ROOT=$(dirname "${GDRIVE_VOL_MOUNT_POINT}")

    cp ~/credentials.json "${GDRIVE_VOL_MOUNT_POINT}"
    chcon -R unconfined_u:object_r:container_file_t:s0 "${GDRIVE_VOL_MOUNT_POINT}"
    podman run --rm -it -v maw-gdrive-creds:/app/secrets/ d0whc3r/gdrive -l
    # follow prompt to view the google consent screen, then copy the code to the terminal to finalize getting the token
    # if everything worked, you should see a list of files from gdrive
}

create_volume_certbot_certs() {
    local VOL_NAME=${1}

    create_volume ${VOL_NAME}

    if [ $? -eq 1 ]; then
        return
    fi

    if [ ! -d /etc/letsencrypt ]; then
        return
    fi

    echo "    - copying lets encrypt data to volume"

    CERTBOT_VOL_MOUNT_POINT=$(podman volume inspect "${VOL_NAME}" | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")
    CERTBOT_VOL_MOUNT_ROOT=$(dirname "${CERTBOT_VOL_MOUNT_POINT}")

    sudo cp -R /etc/letsencrypt/* "${CERTBOT_VOL_MOUNT_POINT}"
    sudo chown -R "${USER}":"${USER}" "${CERTBOT_VOL_MOUNT_ROOT}"
    sudo chcon -R unconfined_u:object_r:container_file_t:s0 "${CERTBOT_VOL_MOUNT_POINT}"
}

create_volumes() {
    local ENV_NAME=${1}

    create_volume maw-certs
    create_volume maw-postgres-backup
    create_volume maw-uploads
    create_volume maw-api-dataprotection
    create_volume maw-auth-dataprotection
    create_volume maw-www-dataprotection
    create_volume maw-reverse-geocode

    create_volume_postgres maw-postgres
    create_volume_solr maw-solr
    create_volume_google_creds maw-google-creds
    create_volume_gdrive_creds maw-gdrive-creds

    if [ "${ENV_NAME}" = 'prod' ]; then
        create_volume maw-certbot-validation
        create_volume_certbot_certs maw-certbot-certs
    fi
}

init_certs() {
    local CERT_IMAGE=${1}

    podman run -it --rm -v maw-certs:/certs:rw,z "${CERT_IMAGE}"

    echo "    - dev certs created"
}

# for development, the local machine will need to trust the certs in order to allow browsers to accept these self signed certs.
# this is not needed for prod, as that will use proper certs from let's encrypt
trust_dev_certs() {
    local CERT_MOUNT_POINT=$(podman volume inspect maw-certs | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")

    sudo find "${CERT_MOUNT_POINT}/internal/ca" -type f -name *.crt -exec cp {} /usr/share/pki/ca-trust-source/anchors/ \;
    sudo update-ca-trust

    echo "    - dev certs are now trusted on local machine"
}

# this will update the firewall to forward requests to the default 80/443 to 8080 and 8443 where the containers will be listening on the host
# the containers do not bind to 80 or 443 as that would require additional perms as we run *rootless* containers via podman
map_default_ports_to_container_ports() {
    # redirect port 80 and 443 to 8080 and 8443 respectively (temporarily - add --permanent to change)
    # https://stackoverflow.com/questions/413807/is-there-a-way-for-non-root-processes-to-bind-to-privileged-ports-on-linux/31795603#31795603
    # https://serverfault.com/questions/654102/forwarding-ports-on-centos-7/713849

    # the following works for external access - run 2x for current runtime and to apply permanently
    sudo firewall-cmd --add-masquerade
    sudo firewall-cmd --add-masquerade --permanent

    sudo firewall-cmd --add-forward-port=port=80:proto=tcp:toport=8080
    sudo firewall-cmd --add-forward-port=port=80:proto=tcp:toport=8080 --permanent

    sudo firewall-cmd --add-forward-port=port=443:proto=tcp:toport=8443
    sudo firewall-cmd --add-forward-port=port=443:proto=tcp:toport=8443 --permanent

    # the following is for access on localhost
    sudo iptables -A OUTPUT -t nat -p tcp --destination 127.0.0.1 --dport 80 -j REDIRECT --to-port 8080
    sudo iptables -A OUTPUT -t nat -p tcp --destination 127.0.0.1 --dport 443 -j REDIRECT --to-port 8443

    # persist the iptables rules across reboots
    sudo service iptables save

    echo "    - firewall is now forwarding requests from 80/443 to 8080/8443"
}

create_containers() {
    local ENV_NAME=${1}
    local POD_NAME=${2}
    local ENV_FILE_DIR=${3}

    if [ ${ENV_NAME} = 'dev' ]; then
        local C_SOLR='localhost/maw-solr-dev:latest'
        local C_PHOTOS='localhost/maw-photos-dev:latest'
        local C_FILES='localhost/maw-files-dev:latest'
        local C_AUTH='localhost/maw-auth-dev:latest'
        local C_API='localhost/maw-api-dev:latest'
        local C_WWW='localhost/maw-www-dev:latest'
        local C_GATEWAY='localhost/maw-gateway-dev:latest'
    else
        local C_SOLR='docker.io/aerisg222/maw-solr:latest'
        local C_PHOTOS='docker.io/aerisg222/maw-photos:latest'
        local C_FILES='docker.io/aerisg222/maw-files:latest'
        local C_AUTH='docker.io/aerisg222/maw-auth:latest'
        local C_API='docker.io/aerisg222/maw-api:latest'
        local C_WWW='docker.io/aerisg222/maw-www:latest'
        local C_GATEWAY='docker.io/aerisg222/maw-gateway:latest'
    fi

    podman container inspect maw-postgres > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-postgres container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-postgres \
            --volume maw-postgres.13.2:/var/lib/postgresql/data:rw,z \
            postgres:13.2
    fi

    podman container inspect maw-solr > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-solr container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-solr \
            --volume maw-solr:/var/solr:rw,z \
            --label "io.containers.autoupdate=image" \
            "${C_SOLR}"
    fi

    podman container inspect maw-photos > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-photos container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-photos \
            --volume maw-certs:/certs:ro,z \
            --label "io.containers.autoupdate=image" \
            "${C_PHOTOS}"
    fi

    podman container inspect maw-files > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-files container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-files \
            --volume maw-certs:/certs:ro,z \
            --label "io.containers.autoupdate=image" \
            "${C_FILES}"
    fi

    podman container inspect maw-auth > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-auth container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-auth \
            --volume maw-certs:/certs:ro,z \
            --volume maw-auth-dataprotection:/dataprotection:rw,Z \
            --volume maw-google-creds:/google-creds:ro,z \
            --label "io.containers.autoupdate=image" \
            --env-file "${ENV_FILE_DIR}/maw-auth.env" \
            "${C_AUTH}"
    fi

    podman container inspect maw-api > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-api container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-api \
            --volume maw-certs:/certs:ro,z \
            --volume maw-api-dataprotection:/dataprotection:rw,Z \
            --volume maw-uploads:/maw-uploads:rw,z \
            --volume /srv/www/website_assets/images:/srv/www/website_assets/images:ro \
            --label "io.containers.autoupdate=image" \
            --security-opt label=disable \
            --env-file "${ENV_FILE_DIR}/maw-api.env" \
            "${C_API}"
    fi

    podman container inspect maw-www > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo "    - creating maw-www container"

        podman create \
            --pod "${POD_NAME}" \
            --name maw-www \
            --volume maw-certs:/certs:ro,z \
            --volume maw-www-dataprotection:/dataprotection:rw,Z \
            --volume maw-google-creds:/google-creds:ro,z \
            --volume /srv/www/website_assets/images:/srv/www/website_assets/images:ro \
            --volume /srv/www/website_assets/movies:/srv/www/website_assets/movies:ro \
            --label "io.containers.autoupdate=image" \
            --security-opt label=disable \
            --env-file "${ENV_FILE_DIR}/maw-www.env" \
            "${C_WWW}"
    fi

    if [ ${ENV_NAME} = 'dev' ]; then
        podman container inspect maw-gateway > /dev/null 2>&1

        if [ $? -ne 0 ]; then
            echo "    - creating maw-gateway container"

            podman create \
                --pod "${POD_NAME}" \
                --name maw-gateway \
                --volume maw-certs:/certs:ro,z \
                --volume /srv/www/website_assets:/assets:ro \
                --label "io.containers.autoupdate=image" \
                --security-opt label=disable \
                "${C_GATEWAY}"
        fi
    else
        # for prod, we need a slightly different setup for the gateway seeing we need to support
        # official certs from Let's Encrypt.  as such, run the gateway as follows:

        podman container inspect maw-gateway > /dev/null 2>&1

        if [ $? -ne 0 ]; then
            echo "    - creating maw-gateway container"

            podman create \
                --pod "${POD_NAME}" \
                --name maw-gateway \
                --volume maw-certs:/certs:ro,z \
                --volume maw-certbot-validation:/var/www/certbot:ro,z \
                --volume maw-certbot-certs:/etc/letsencrypt:ro,z \
                --volume /srv/www/website_assets:/assets:ro \
                --label "io.containers.autoupdate=image" \
                --security-opt label=disable \
                "${C_GATEWAY}"
        fi
    fi
}

create_certbot_job() {
    local POD_NAME=${1}
    local SVC=~/.config/systemd/user/certbot-renew.service
    local TIMER=~/.config/systemd/user/certbot-renew.timer

    echo '    - creating certbot renew job!'

    echo "[Unit]" > ${SVC}
    echo "Description=Renew Let's Encrypt certificates for mikeandwan.us" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=podman run -it --rm --pod maw-pod --volume maw-certbot-validation:/var/www/certbot:rw,z --volume maw-certbot-certs:/etc/letsencrypt:rw,z docker.io/certbot/certbot:latest renew --agree-tos" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run certbot-renew twice a day" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=0/12:00:00" >> ${TIMER}
    echo "RandomizedDelaySec=1h" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_postgres_job() {
    local POD_NAME=${1}
    local ENV_FILE_DIR=${2}
    local SVC=~/.config/systemd/user/postgres-maintenance.service
    local TIMER=~/.config/systemd/user/postgres-maintenance.timer

    echo '    - creating postgres maintenance job!'

    echo "[Unit]" > ${SVC}
    echo "Description=PostgreSQL backup and optimization jobs for mikeandwan.us databases" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=podman run -it --rm --pod maw-pod --volume maw-postgres-backup:/pg_backup:rw,z --env-file ${ENV_FILE_DIR}/maw-postgres-backup.env docker.io/aerisg222/maw-postgres-maintenance:latest" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run postgres-maintenance once a day" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=01:00:00" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_reverse_geocode_job() {
    local POD_NAME=${1}
    local ENV_FILE_DIR=${2}
    local SVC=~/.config/systemd/user/reverse-geocode.service
    local TIMER=~/.config/systemd/user/reverse-geocode.timer

    echo '    - creating reverse geocode job!'

    echo "[Unit]" > ${SVC}
    echo "Description=Reverse Geocode job for mikeandwan.us" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=podman run -it --rm --pod maw-pod --volume maw-reverse-geocode:/results:rw,z --env-file ${ENV_FILE_DIR}/maw-reverse-geocode.env docker.io/aerisg222/maw-reverse-geocode:latest" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run reverse-geocode once a day" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=23:30:00" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_solr_reindex_job() {
    local POD_NAME=${1}
    local SVC=~/.config/systemd/user/solr-reindex.service
    local TIMER=~/.config/systemd/user/solr-reindex.timer

    echo '    - creating solr reindex job!'

    echo "[Unit]" > ${SVC}
    echo "Description=Solr Reindex job for mikeandwan.us" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=podman run -it --rm --pod maw-pod docker.io/aerisg222/maw-solr-reindex:latest" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run solr-reindex once a day" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=02:30:00" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_remote_archive_job_daily() {
    local POD_NAME=${1}
    local SVC=~/.config/systemd/user/remote-archive-daily.service
    local TIMER=~/.config/systemd/user/remote-archive-daily.timer

    echo '    - creating remote archive daily job!'

    echo "[Unit]" > ${SVC}
    echo "Description=Daily remote archive job for mikeandwan.us" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=/home/${USER}/remote-archive.sh daily" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run remote archive once a day" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=01:30:00" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_remote_archive_job_monthly() {
    local POD_NAME=${1}
    local SVC=~/.config/systemd/user/remote-archive-monthly.service
    local TIMER=~/.config/systemd/user/remote-archive-monthly.timer

    echo '    - creating remote archive monthly job!'

    echo "[Unit]" > ${SVC}
    echo "Description=Long term remote archive job for mikeandwan.us" >> ${SVC}
    echo "" >> ${SVC}
    echo "[Service]" >> ${SVC}
    echo "Type=oneshot" >> ${SVC}
    echo "ExecStart=/home/${USER}/remote-archive.sh monthly" >> ${SVC}

    echo "[Unit]" > ${TIMER}
    echo "Description=Run long term remote archive once a month" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Timer]" >> ${TIMER}
    echo "OnCalendar=*-*-01 01:45:00" >> ${TIMER}
    echo "" >> ${TIMER}
    echo "[Install]" >> ${TIMER}
    echo "WantedBy=timers.target" >> ${TIMER}
}

create_remote_archive_job() {
    create_remote_archive_job_daily ${POD_NAME}
    create_remote_archive_job_monthly ${POD_NAME}
}

start_enable_certbot_job() {
    systemctl --user start certbot-renew.timer
    systemctl --user enable certbot-renew.timer
}

start_enable_postgres_job() {
    systemctl --user start postgres-maintenance.timer
    systemctl --user enable postgres-maintenance.timer
}

start_enable_reverse_geocode_job() {
    systemctl --user start reverse-geocode.timer
    systemctl --user enable reverse-geocode.timer
}

start_enable_solr_reindex_job() {
    systemctl --user start solr-reindex.timer
    systemctl --user enable solr-reindex.timer
}

start_enable_solr_reindex_job() {
    systemctl --user start remote-archive-daily.timer
    systemctl --user start remote-archive-monthly.timer

    systemctl --user enable remote-archive-daily.timer
    systemctl --user enable remote-archive-monthly.timer
}

configure_systemd() {
    local POD_NAME=${1}
    local ENV_FILE_DIR=${2}

    mkdir ~/.config/systemd/user

    pushd ~/.config/systemd/user

    podman generate systemd --files --name --new "${POD_NAME}"

    create_certbot_job "${POD_NAME}"
    create_postgres_job "${POD_NAME}" "${ENV_FILE_DIR}"
    create_reverse_geocode_job "${POD_NAME}" "${ENV_FILE_DIR}"
    create_solr_reindex_job "${POD_NAME}"
    create_remote_archive_job "${POD_NAME}"

    popd

    systemctl --user daemon-reload
    systemctl --user start "pod-${POD_NAME}.service"
    systemctl --user enable "pod-${POD_NAME}.service"

    start_enable_certbot_job
    start_enable_postgres_job
    start_enable_reverse_geocode_job
    start_enable_solr_reindex_job
    start_enable_remote_archive_job

    # allow services to run w/o user logged in
    sudo loginctl enable-linger "${USER}"

    echo '*********************'
    echo ' Add Before entries to systemd container service units so postgres and solr start first and gateway starts last'
    echo '*********************'
}

build_pod_dev() {
    local POD_NAME="maw-pod-dev"

    show_info ${POD_NAME}

    create_pod "${POD_NAME}"
    create_volumes 'dev'
    init_certs 'localhost/maw-certs-dev'
    trust_dev_certs
    map_default_ports_to_container_ports
    create_containers 'dev' "${POD_NAME}" "/home/mmorano/git"

    show_done
}

build_pod_prod() {
    local POD_NAME="maw-pod"

    show_info ${POD_NAME}

    create_pod "${POD_NAME}"
    create_volumes 'prod'
    init_certs 'aerisg222/maw-certs'
    map_default_ports_to_container_ports
    create_containers 'prod' "${POD_NAME}" "/home/svc_www_maw"
    configure_systemd "${POD_NAME}" "/home/svc_www_maw"

    show_done
}

show_info() {
    local POD_NAME=$1

    echo
    echo "This script will initialize the container environment for mikeandwan.us in pod *** ${POD_NAME} ***"
    echo '  - this assumes *all* images are available - either local for dev or in dockerhub for prod'
    echo '  - run this script with the user the site should run as (i.e. svc_www_maw)'
    echo '  - to execute, please specify "dev" or "prod" to indicate which environment to create'
    echo '  - the script will automatically migrate current solr and postgres instances'
    echo '  - the script will also prepare the gdrive archive - but you must first place the containers.json file in ~/'
    echo '  - you should also copy the remote-archive.sh to the service accounts home directory, as this is referenced by the systemd service'
    echo

    read -n 1 -r -s -p $'Press enter to continue...\n'

    echo
    echo "running all steps to create ${POD_NAME}"
}

show_done() {
    echo "    - completed"
    echo
}

if   [ "${1}" = 'dev' ]; then
    build_pod_dev
elif [ "${1}" = 'prod' ]; then
    build_pod_prod
else
    echo 'Please specify if you would like to build a "prod" or "dev" pod'
fi
