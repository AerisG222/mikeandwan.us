# Server Configuration

This document tries to walk through the steps required to get this website up and running.  This is
currently a mix and match of other notes I had been keeping about setting up the production server,
but will try to get this well organized below to make it easy to follow.


## Server Setup

1. Install Fedora 23 server from USB, using Fedora Server environment group
2. Apply any updates: `sudo dnf upgrade`
3. Edit /etc/hostname  [change to tifa.mikeandwan.us]
4. Edit /etc/sysconfig/network-scripts/ifcfg-enp0s25 (or whatever the interface is)
    - need to update to provide configuration for static IP details. for this instance, we added/updated the following:
        ```
        BOOTPROTO=none
        IPADDR0=192.168.1.xxx  (replace xxx)
        PREFIX0=24
        GATEWAY0=192.168.1.1
        DNS0=192.168.1.1
        ```
5. Ensure network is configured to start `systemctl list-units *net*`
    - if not enabled:  `systemctl enable network.service`
6. Reboot (get on latest kernel / verify networking)
7. Ensure network is connected properly (`ifconfig -a`)
8. Ensure /etc/resolv.conf is configured right (should have search and nameserver set right)
9. Optimize networking - edit /etc/sysctl.conf
    - [nixCraft Guide - Nginx Hardening](http://www.cyberciti.biz/tips/linux-unix-bsd-nginx-webserver-security.html)
    - [nixCraft Guide - Congestion Control](https://www.cyberciti.biz/cloud-computing/increase-your-linux-server-internet-speed-with-tcp-bbr-congestion-control/)
10. Update firewall to allow http/https
    ```
    firewall-cmd --zone=FedoraServer --list-allow
    firewall-cmd --permanent --zone=FedoraServer --add-service=https
    firewall-cmd --permanent --zone=FedoraServer --add-service=http
    systemctl restart firewalld.service
    ```
11. Install tool to update DNS entries if ISP IP changes
    - `dnf install ddclient`
    - update /etc/ddclient.conf
        ```
        daemon=600
        ssl=yes

        ##
        ## NameCheap (namecheap.com)
        ##
        protocol=namecheap,                             \
        server=dynamicdns.park-your-domain.com,         \
        login=YOUR_LOGIN,                               \
        password=YOUR_PASSWORD                          \
        www,@,api,auth
        ```
    - start and enable ddclient
        ```
        systemctl start ddclient.service
        systemctl enable ddclient.service
        ```
12. Harden sshd
    - edit /etc/ssh/sshd_config and set the following:
        ```
        ServerKeyBits to 2048
        PermitRootLogin no
        AllowUsers mmorano
        HostbasedAuthentication no
        X11Forwarding no
        AllowTcpForwarding no
        ```
    - `rm /etc/ssh/ssh_host *`
    - configure ssh keys to avoid having to log in with pwds
        - https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys--2
        - https://help.github.com/articles/generating-an-ssh-key/
13. Install tool to limit brute force attacks
    - [Reference Guide](http://www.cyberciti.biz/faq/rhel-linux-block-ssh-dictionary-brute-force-attacks/)
    - `dnf install denyhosts`
    - edit /etc/hosts.allow - ensure workstation can access server
        ```
        sshd: 192.168.1.12
        ```
    - Start and enable denyhosts
        ```
        systemctl start denyhosts.service
        systemctl enable denyhosts.service
        ```
14. Add service account
    - `useradd -m -r svc_www_maw`
15. Update group associations
    - `vigr`
        - add svc_www_maw as a member of nginx
        - add nginx as a member of svc_www_maw
16. Install remaining dependencies
    - `sudo dnf install imagemagick-devel`


## PostgreSQL

- Adapted from this [guide](https://fedoraproject.org/wiki/PostgreSQL)
    ```
    sudo dnf install postgresql postgresql-server
    sudo postgresql-setup --initdb --unit postgresql
    sudo systemctl start postgresql.service
    sudo systemctl enable postgresql.service
    ```
- Create admin account
    ```
    su - postgres
    psql
    CREATE USER mmorano;
    ALTER USER mmorano WITH SUPERUSER;
    ```
- Create database [and migrate data from mysql]
    - `000/create_maw_website.sh`
- Copy update scripts to server
    - `rsync -a -f"+ */" -f"+ *.sql" -f"- *" . mmorano@tifa:/home/mmorano/deploy/update_scripts`
- Run fixups - cd to root directory containing year folders
    ```
    # 0: cd to the root directory containing the year folders (images_new)

    # 1: correct sigmoidal_contrast_adjustment column name
    find . -type f -exec sed -i 's/ sigmoidal_adjustment / sigmoidal_contrast_adjustment /g' {} +

    # 2: correct for case sensitivity
    find . -type f -exec sed -i "s/WHERE lg_path = '\(.*\)'/WHERE UPPER(lg_path) = UPPER('\1')/g" {} +

    # 3: create temp index to support following case insensitive lookups
    CREATE INDEX photo_upper_lg_path ON photo.photo (UPPER(lg_path));
    CREATE INDEX photo_src_path ON photo.photo (src_path);

    # 4: apply all sql updates to database (all glacier sql files are in the root, so skip those - need to do in this order so src_path is properly set first):
    find . -mindepth 2 -type f -execdir psql -d maw_website -f {} \;

    # 5: apply glacier backup updates to database:
    find . -maxdepth 1 -type f -execdir psql -d maw_website -f {} \;

    # 6: confirm all photos have backup info
    select count(1) from photo.photo where aws_archive_id is null;

    # 7: drop temp index
    DROP INDEX photo.photo_upper_lg_path;
    DROP INDEX photo.photo_src_path;
    ```


## .NET Core

- Follow [guide](https://www.microsoft.com/net/learn/get-started/linuxfedora)


## Nginx

1. `sudo dnf install nginx`
2. Update SELinux to connect to Kestrel (if using tcp for kestrel)
    - `setsebool -P httpd_can_network_connect 1`
3. Start and enable Nginx to start at boot
    ```
    sudo systemctl start nginx.service
    sudo systemctl enable nginx.service
    ```
4. Copy the nginx configuration files from the src directory, and put on the server under /etc/nginx
5. Add the sites to nginx:
    - `ln -s /etc/nginx/sites-available/maw_api.conf /etc/nginx/conf.d/maw_api.conf`
    - `ln -s /etc/nginx/sites-available/maw_auth.conf /etc/nginx/conf.d/maw_auth.conf`
    - `ln -s /etc/nginx/sites-available/maw_www.conf /etc/nginx/conf.d/maw_www.conf`
6. `sudo dnf install certbot-nginx`
    - `certbot certonly -d mikeandwan.us -d www.mikeandwan.us --nginx`
    - `certbot certonly -d api.mikeandwan.us --nginx`
    - `certbot certonly -d auth.mikeandwan.us --nginx`
7. configure certbot to run 2 times a day to update if needed (via systemd)
    - [service](systemd/certbot-renew.service)
    - [timer](systemd/certbot-renew.timer)
8. You might need to configure SE Linux rules:
    - reference: https://docs-old.fedoraproject.org/en-US/Fedora/12/html/Security-Enhanced_Linux/sect-Security-Enhanced_Linux-Fixing_Problems-Allowing_Access_audit2allow.html
    - `grep cert_t /var/log/audit/audit.log | audit2allow -M maw_us_certwatch`
9. Deploy site to server by using tools/publish_website.sh
10. Now update selinux to allow nginx to access the wwwroot directory (labels might not have been copied)
    ```
    restorecon -Rv /srv/www/mikeandwan.us/wwwroot/
    restorecon -Rv /etc/nginx/certs
    restorecon -Rv /etc/nginx/sites-available
    ```
11. Deploy assets to server, then update SELinux labels
    - `restorecon -Rv /srv/www/website_assets/`
12. Prepare log directory
    ```
    mkdir /var/log/nginx/mikeandwan.us
    chmod g+rx /var/log/nginx
    chown nginx:nginx /var/log/nginx/mikeandwan.us
    chmod g+rwx /var/log/nginx/mikeandwan.us
    ```
13. Reference: [https://docs.asp.net/en/latest/publishing/linuxproduction.html](ASP.Net Linux Production)


## Systemd

This will ensure that the backend application server (Kestrel / your website)
runs at system startup.  Also, if the process dies, this will restart the process.  Depending
on whether you want Nginx to connect to kestrel via TCP or Unix sockets will drive some of the
decisions below, which are called out separately.  Systemd will also collect
output to STDOUT and include it in the log / journal.

1. Create service unit file
    - Create [/etc/systemd/maw_us.service](systemd/maw_us.service)
2. Start and enable the webapp to start at boot
    ```
    sudo systemctl start maw_us.service
    sudo systemctl enable maw_us.service
    ```
3. Update SELinux for unix sockets
    - The process will fail when trying to use unix sockets due to SELinux preventing the connection from nginx to the kestrel unix socket.
      To fix this, do the following:
        - `audit2allow -i /var/log/audit/audit.log`
        - Review the output, you should see something in regards to needing to allow `file_sock`
        - Add a new SELinux policy to allow nginx to connect to kestrel
            ```
            audit2allow -i /var/log/audit/audit.log -M kestrel
            mv kestrel* /usr/share/selinux/targeted/
            semodule -i /usr/share/selinux/targeted/kestrel.pp
            ```


## Scheduled Maintenance

1. Download [gdrive](https://github.com/prasmussen/gdrive) that can be used to upload period db backups to Google Drive
2. Configure routine maintenance (db backups)
    - Run `crontab -e` and enter the following [tasks](crontab/crontab.conf)


## Upgrade Fedora

1. Follow instructions to upgrade: [DNF_system_upgrade](https://fedoraproject.org/wiki/DNF_system_upgrade)
2. `sudo dnf install rpmconf`
3. `sudo rpmconf -a`: go through and clean up old configs
4. `sudo dnf install postgresql-upgrade`
5. `sudo postgresql-setup --upgrade`
6. Re-apply configurations
    - `diff /var/lib/pgsql/data/pg_hba.conf /var/lib/pgsql/data-old/pg_hba.conf`
    - `diff /var/lib/pgsql/data/postgresql.conf /var/lib/pgsql/data-old/postgresql.conf`
7. `sudo systemctl start postgresql.service`
8. `sudo rm -rf /opt/dotnet`  (preview versions were causing issues for me)
9. Follow instructions on installing for Fedora 24: [fedora 24 instructions](https://www.microsoft.com/net/core#linuxfedora)
10. SE Linux updates
    - nginx would not start, run `systmctl status nginx.service` to see what is going on
    - In the latest case, I saw an AVC denial for the /var/log/nginx/error.log
    - Ran `grep nginx /var/log/audit/audit.log | audit2allow -M nginx`, which said to run the following:
      - `semodule -i nginx.pp`, which then allowed nginx to start
11. Postgres updates
    - `postgresql-setup --upgrade`


## GMail SSL/TLS updates

.Net core uses Openssl on linux for SSL/TLS connections.  The certs for gmail are not typically included
with that distribution, at least it hasn't on my Fedora boxes.  Here are the steps needed to configure openssl
to allow for the certs to be verified:

1. List all certs / intermediate certs for gmail:
    - `openssl s_client -starttls smtp -connect smtp.gmail.com:587 -showcerts`
        - This currently results in 3 certs in the list, one for smtp.gmail.com, one for Google CA, and one for GeoTrust
        - Copy each PEM formatted certificate (between the -- BEGIN CERTIFICATE -- ... -- END CERTIFICATE -- delimiters) as its own file
        - You can then use openssl to verify / view these certs
            - `openssl verify filename.crt`
            - `openssl x509 -in filename.crt -text`
    - I was getting errors on the GeoTrust cert, and thought it was because I did not have the root CA - the Equifax Secure Certificate Authority.
    - I then searched for this cert and found it, which seemed to be the missing piece in getting this to work
2. Copy all these certs (total of 4, see the gmail_certs folder for the ones I am currently using) to `/usr/share/pki/ca-trust-source/anchors`
3. `update-ca-trust`
4. `update-ca-trust extract`
5. `killall dotnet` / reboot / restart the webapp
6. this should now work!


## GMail API

As an alternative to SMTP above, we can leverage Google REST APIs to interface with GMail to send mails.  For this, we need to setup
a service account.  Necessary details for this configuration can be found [here](https://developers.google.com/identity/protocols/OAuth2ServiceAccount).

The basic steps are as follows:

    1. create a new Service Account, saving the credential json file securely, and enabling Domain Wide Delegation
    2. configure the service account to enable the Gmail Send API through the [Admin Console](https://admin.google.com/AdminHome)
        - Security / Advanced / Manage API Client Access
        - Add API Scope: https://www.googleapis.com/auth/gmail.send
    3. copy the credentials json file to the service account homedir on the server, and make sure only that user has access
    4. add environment variable named `GOOGLE_APPLICATION_CREDENTIALS` to point to the location of the json credential file downloaded in step 1

The benefit of the API rather than SMTP is that it is much more performant in my tests so far, and more secure as we could disable 'Less secure apps'
for the user.


## Notes for copying assets for dev

```
mkdir -p /srv/www/website_assets
cd /srv/www/website_assets
rsync --progress -a -f "+ */" -f "+ */xs/*" -f "- *"   mmorano@tifa:/srv/www/website_assets/images .
rsync --progress -a -f "+ */" -f "+ */thumbnails/*" -f "- *"   mmorano@tifa:/srv/www/website_assets/movies .
```
