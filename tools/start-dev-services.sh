#!/bin/bash
systemctl --user start pod-dev-maw-pod.service

gnome-terminal \
    --tab \
    --working-directory /home/mmorano/git/mikeandwan.us/tools \
    --title media \
    -- bash -c './unlink_media.sh && sleep 30s && ./link_media.sh'

gnome-terminal \
    --tab \
    --working-directory /home/mmorano/git/mikeandwan.us/src/auth \
    --title auth \
    -- dotnet run;

gnome-terminal \
    --tab \
    --working-directory /home/mmorano/git/mikeandwan.us/src/api \
    --title api \
    -- dotnet run;

gnome-terminal \
    --tab \
    --working-directory /home/mmorano/git/mikeandwan.us/src/www \
    --title www \
    -- dotnet run
