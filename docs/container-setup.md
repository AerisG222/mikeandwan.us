# podman config

The following steps will walk through the process of configuring podman for hosting all the containers for mikeandwan.us.

## PROD CONFIG

- take db backups

- add user/group to subuid and subgid mapping:
  - `vi /etc/subuid`
  - `vi /etc/subgid`
  - add the following to both: `svc_www_maw:240000:6553`

- https://github.com/containers/libpod/issues/5903
- https://github.com/containers/libpod/issues/6084

- add service account to sshd - they need a systemd user context:
    - `vi /etc/ssh/sshd_config` - add user to AllowUsers
    - `systemctl --user`: should return info and not an error message
- login as service account directly via ssh - which will establish the systemctl session
- run `loginctl enable-linger`
- run `build-pod.sh prod`

- enable service account to use sudo (for setup script)
  - `vigr` - add user to wheel group

- modify the postgres config pg_hba.conf in the volume to allow md5 auth for all ip connections

## SOLR

After setting up the Solr container, it would not start because it kept on indicating there was a permission issue
when copying solr.xml.  To address this, we needed to update the ownership of the files on the volume to the
user running solr in the container.  By looking at the [Dockerfile](https://github.com/docker-solr/docker-solr/blob/master/Dockerfile.template)
used to build the solr container, we can see that it defines the solr user with an UID and GID of 8983.  Therefore,
to properly set the permissions on the volume, we need to execute the following command:

`podman unshare chown -R 8983:8983 /srv/podman/1000/storage/volumes/solr`

Once that is executed, running the container as follow works as expected

`podman run -it --rm -v solr:/var/solr:rw,z -p 8983:8983 localhost/solr`

There is a great [blog post](https://www.redhat.com/sysadmin/rootless-podman-makes-sense) describing this permission issue.

## POSTGRES

Taking our information from setting up SOLR, we can look at the postgres dockerfile, and see it is using UID/GID  = 999,
so we will issue the same commands above for the postgres volume, and the postgres user/group IDs:

``` bash
sudo cp -R /var/lib/pgsql/data /srv/podman/1000/storage/volumes/postgres/_data
sudo chown -R mmorano:mmorano /srv/podman/1000/storage/volumes/postgres
podman unshare chown -R 999:999 /srv/podman/1000/storage/volumes/postgres
podman run -it --rm -v postgres:/var/lib/postgresql/data:rw,z -p 5432:5432 postgres:12.2
```

## Container to Container Communication

After getting postgres running, we confirmed that our data was available through our mount.
This was done by using psql on the host to connect to the server running in the container.
However, when we tried to then initiate the same test from using psql in a container, it was
not able to locate the postgres service.

After some searching we came across the following [article](https://www.redhat.com/sysadmin/container-networking-podman).
Reading through to 'Communicate between two rootless containers' we find the information
needed to connect containers that are not running in the same pod.  Namely, confirming
the port mapping with `podman port -l`, and then identifying the ip that is used for the
container network via `ip addr show` which showed a device `eno1` with an ip `192.168.1.211`.

Given that information, we could then connect to postgres from a different container as follows:

`podman run -it --rm postgres:12.2 psql -h 192.168.1.211 -U svc_www_maw -d maw_website`

## Container to Container Communication (part 2)

A simpler approach of allowing containers to communicate to each other is by running
containers within the same pod.  There is a good [writeup](https://developers.redhat.com/blog/2019/01/15/podman-managing-containers-pods/)
on this approach.  What is particularly interesting, is you do not have to expose any
ports to the underlying host, which means that you can force users to have to run
containers in the pod to access different services.

Using the sample above of trying to connect to postgres from another container, we can do this
with pods by the following:

``` bash
podman pod create -n testpod
podman run -it --pod testpod --rm -v postgres:/var/lib/postgresql/data:rw,z postgres:12.2
podman run -it --pod testpod --rm postgres:12.2 psql -h localhost -U svc_www_maw -d maw_website
```

Those will all work, as postgres and the client are running within the same pod - so containers can
just reference 'localhost' to communicate with each other.  However, if we try to connect to
postres from the host: `psql -h localhost -U svc_www_maw -d maw_website', or using the IP from
the previous section, it *cannot* connect!

## Common POD

In order for all the services to be able to connect to one another, we will setup a pod for
all the services to live within.  In this way, services can reference one another in the pod
simply by localhost and the appropriate port - which is quite convenient for our local hosting model.

One trick with this, is that we would like to fully run a rootless pod environment.  However,
to bind to port 80 and 443, this requires root access.  Given this, we will use no-default ports
for the gateway, and will use port forwarding (from router in prod, and via SSH local forwarding for dev)
to allow these to run in a rootless fashion, which is also needed so they can live within the same pod.

To setup the pod, we need to run the following:

``` bash
# create pod
podman pod create --name maw-pod -p 8080:80 -p 8443:443

# add containers to pod
mawphotos=$(podman create --pod maw-pod -v certs:/certs:ro,z localhost/maw-photos-dev)
mawgateway=$(podman create --pod maw-pod -v certs:/certs:ro,z localhost/maw-gateway-dev)

# start containers
podman start $mawphotos
podman start $mawgateway
```

## OLD

Originally, there was interest in moving default storage locations outside of the homedir.  While
this had worked originally, we will start to use the defaults going forward, and will try to define
volumes that map to images and movies outside of the default volume location.

Below are the original steps to define the custom location (which is likely to change in the future
when containers.conf is fully supported).

This was originally achieved thanks to [this great blog post](https://qulogic.gitlab.io/posts/2019-10-20-migrating-to-podman/).

### Step 0: Cleanup (if needed)

``` bash
rm -rf ~/.local/share/containers
rm -rf ~/.config/containers
```

### Step 1: Create new storage location (as root)

``` bash
mkdir -p /srv/podman/1000
chown 1000:1000 /srv/podman/1000
chmod 0700 /srv/podman//1000
semanage fcontext -a -e $HOME/.local/share/containers /srv/podman/1000
restorecon -R -v /srv/podman/1000/
```

### Step 2: Update local config files (as user)

``` bash
mkdir -p ~/.config/containers
cd ~/.config/containers
cp /etc/containers/storage.conf .
touch ~/.config/containers/libpod.conf
```

With the above steps completed, now edit storage.conf to override the following values:

``` text
driver = "vfs"
runroot = "/srv/podman/1000/run/storage"
graphroot = "/srv/podman/1000/storage"
```

Finally, enter the following in libpod.conf:

``` text
volume_path = "/srv/podman/1000/storage/volumes"
static_dir = "/srv/podman/1000/storage/libpod"
```

### Step 3: Confirm

Execute the following to ensure that podman is properly loading the configuration:

`podman images ls`

The above command should show an empty list of images - but more importantly, should not show any errors!

Additionally, verify where volumes are created by running:

`podman volume create testvol`

Once that executes, you should see this volume as a new directory under /srv/podman/1000/storage/volumes.
Once verified, remove the test volume with the following command:

`podman volume rm testvol`
