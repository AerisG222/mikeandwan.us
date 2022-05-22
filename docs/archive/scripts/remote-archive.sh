#!/bin/bash
PG_BACKUP_MOUNT_POINT=$(podman volume inspect maw-postgres-backup | python3 -c "import sys, json; print(json.load(sys.stdin)[0]['Mountpoint'])")

IDSRV_DUMP=$(ls -t "${PG_BACKUP_MOUNT_POINT}"/idsrv*.dump | head -n 1)
WWW_DUMP=$(ls -t "${PG_BACKUP_MOUNT_POINT}"/maw_website*.dump | head -n 1)

IDSRV_DUMP=$(basename ${IDSRV_DUMP})
WWW_DUMP=$(basename ${WWW_DUMP})

if [ "${1}" == "daily" ]; then
    echo 'Archiving latest files to gdrive...'

    # we expect to run this shell script from systemd, and then pass in args to the gdrive container
    # (we can't embed this in a container due to how it defines gdrive as the entrypoint rather than cmd)
    podman run -it \
    --pod maw-pod \
    --rm \
        -v maw-postgres-backup:/pg_backup:ro,z \
        -v maw-gdrive-creds:/app/secrets:ro,z \
        d0whc3r/gdrive \
            -b "/pg_backup/${IDSRV_DUMP}" \
            -b "/pg_backup/${WWW_DUMP}" \
            -f pg_backup \
            -d pg_backup=10d
fi

if [ "${1}" == "monthly" ]; then
    echo 'Archiving start of month files to gdrive...'

    # archive monthly files for up to 2 years
    podman run -it \
     --pod maw-pod \
      --rm \
        -v maw-postgres-backup:/pg_backup:ro,z \
        -v maw-gdrive-creds:/app/secrets:ro,z \
           d0whc3r/gdrive \
             -b "/pg_backup/${IDSRV_DUMP}" \
             -b "/pg_backup/${WWW_DUMP}" \
             -f pg_backup_monthly \
             -d pg_backup_monthly=2y
fi
