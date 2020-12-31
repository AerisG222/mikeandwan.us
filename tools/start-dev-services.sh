#!/bin/bash
gnome-terminal \
    --tab --working-directory=/home/mmorano/git/mikeandwan.us/tools    --title=media --command="bash -c './unlink_media.sh && sleep 30s && ./link_media.sh';bash" \
    --tab --working-directory=/home/mmorano/git/mikeandwan.us/src/auth --title=auth  --command="dotnet run" \
    --tab --working-directory=/home/mmorano/git/mikeandwan.us/src/api  --title=api   --command="dotnet run" \
    --tab --working-directory=/home/mmorano/git/mikeandwan.us/src/www i --title=www  --command="dotnet run" \
