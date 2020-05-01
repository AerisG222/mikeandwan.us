# podman config

The following steps will walk through the process of configuring podman for hosting all the containers for mikeandwan.us.
Rather than accepting the default location under the user homedir, the steps below also move the files for podman under
the preferred /srv directory (another suitable option could be the /var directory).

This was originally achieved thanks to [this great blog post](https://qulogic.gitlab.io/posts/2019-10-20-migrating-to-podman/).

## Step 0: Cleanup (if needed)

``` bash
rm -rf ~/.local/share/containers
rm -rf ~/.config/containers
```

## Step 1: Create new storage location (as root)

``` bash
mkdir -p /srv/podman/1000
chown 1000:1000 /srv/podman/1000
chmod 0700 /srv/podman//1000
semanage fcontext -a -e $HOME/.local/share/containers /srv/podman/1000
restorecon -R -v /srv/podman/1000/
```

## Step 2: Update local config files (as user)

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

## Step 3: Confirm

Execute the following to ensure that podman is properly loading the configuration:

`podman images ls`

The above command should show an empty list of images - but more importantly, should not show any errors!

Additionally, verify where volumes are created by running:

`podman volume create testvol`

Once that executes, you should see this volume as a new directory under /srv/podman/1000/storage/volumes.
Once verified, remove the test volume with the following command:

`podman volume rm testvol`

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
