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
    - followed document at [nixCraft Guide](http://www.cyberciti.biz/tips/linux-unix-bsd-nginx-webserver-security.html)
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
        www,@,aeris
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


## .NET Core

- Adapted from this [guide](https://www.microsoft.com/net)
    ```
    sudo dnf install libunwind libicu
    curl -sSL -o dotnet.tar.gz https://go.microsoft.com/fwlink/?LinkID=827531
    sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
    sudo ln -s /opt/dotnet/dotnet /usr/local/bin
    ```


## Nginx

1. `sudo dnf install nginx`
2. Update SELinux to connect to Kestrel (if using tcp for kestrel)
    - `setsebool -P httpd_can_network_connect 1`
3. Start and enable Nginx to start at boot
    ```
    sudo systemctl start nginx.service
    sudo systemctl enable nginx.service
    ```


## Supervisord

This application will ensure that the backend application server (Kestrel / your website)
runs at system startup.  Also, if the process dies, this will restart the process.  Depending
on whether you want Nginx to connect to kestrel via TCP or Unix sockets will drive some of the
decisions below, which are called out separately below.

1. `sudo dnf install supervisord`
2. Configure your webapp / Kestrel
    - For TCP Sockets: 
        - Create config file [/etc/supervisord/mikeandwan.us.ini](supervisord/tcp_mikeandwan.us.ini)
    - For Unix Sockets:
        - `mkdir /var/kestrel`
        - `chown svc_www_maw:svc_www_maw /var/kestrel`
        - `chmod ug+rwx /var/kestrel`
        - Create config file [/etc/supervisord/mikeandwan.us.ini](supervisord/unix_mikeandwan.us.ini)
            - notice that the ASPNETCORE_URLS environment variable is set to define the location of the unix sockets
            - the command now points to the script below, which will ensure the unix socket is cleaned up before starting
            - also, umask is set to 002, which allows the owner and group to read, write, and execute the socket file when created.
              This is needed because we are using 2 different users for the nginx process (nginx), and the kestrel service (svc_www_maw).
              This is why we needed to update nginx to be a member of svc_www_maw in earlier instructions.
        - Create startup script [/home/svc_www_maw/start_mikeandwan.us.sh](svc_www_maw/start_mikeandwan.us.sh)
3. Start and enable supervisord to start at boot
    ```
    sudo systemctl start supervisord.service
    sudo systemctl enable supervisord.service
    ```
4. Update SELinux for unix sockets
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


## Website

1. Create /etc/nginx/certs and prepare cert related files for nginx
    - copy the cert from thawte to this directory (mikeandwan.us.pem)
    - get the intermediary and root CA certificates from thawte
        - if you arent sure which certs to download, run the following command and look for the issuer details, 
          then find this cert on the issuer's site
          `openssl x509 -in mikeandwan.us.pem -text`
    - `cat mikeandwan.us.pem thawte.dv_ssl_ca_g2.pem thawte.root.pem > mikeandwan.us.bundle.pem`
    - `cat thawte.root.pem thawte123.sha2.intermediate.pem > thawte.bundle.pem`
    - `openssl dhparam -out mikeandwan.us.dhparam.pem 2048`
2. Create [/etc/nginx/maw_ssl.conf](nginx/maw_ssl.conf)
3. Create /etc/nginx/sites-available and copy in [mikeandwan.us.conf](nginx/mikeandwan.us.conf)
    - Comment/Uncomment the proxy_pass line based on whether you are using TCP or Unix Sockets to communicate with Kestrel
4. Update [/etc/nginx/mime.types](nginx/mime.types)
5. Add this site to nginx
    - `ln -s /etc/nginx/sites-available/mikeandwan.us.conf /etc/nginx/conf.d/mikeandwan.us.conf`
6. Deploy site to server by using tools/publish_website.sh
7. Now update selinux to allow nginx to access the wwwroot directory (labels might not have been copied)
    ```
    restorecon -Rv /srv/www/mikeandwan.us/wwwroot/
    restorecon -Rv /etc/nginx/certs
    restorecon -Rv /etc/nginx/sites-available
    ```
8. Deploy assets to server, then update SELinux labels
    - `restorecon -Rv /srv/www/website_assets/`


## Scheduled Maintenance

1. Download [gdrive](https://github.com/prasmussen/gdrive) that can be used to upload period db backups to Google Drive
2. Configure routine maintenance (db backups)
    - Run `crontab -e` and enter the following [tasks](crontab/crontab.conf)


## Upgrade Fedora 23 to Fedora 24

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


## Old Notes

The following was needed on earlier versions of the site, but not sure if this is
still needed with .NET Core / ASP.NET Core:

- add the appropriate certificate bits for gmail connectivity (as root)
    - `certmgr -ssl -m smtps://smtp.gmail.com:465`
    - type 'yes' for all prompts
