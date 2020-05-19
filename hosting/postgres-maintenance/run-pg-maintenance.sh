#!/bin/bash
set -e

echo 'Starting backup for maw_website...'
pg_dump -h localhost -U postgres -Fc maw_website > /pg_backup/maw_website.`date +\%Y\%m\%d`.dump

echo 'Starting backup for idsrv...'
pg_dump -h localhost -U postgres -Fc idsrv > /pg_backup/idsrv.`date +\%Y\%m\%d`.dump

echo 'Cleaning old backups...'
find /pg_backup/* -type f -mtime +5 -not -iname *01.dump -delete

echo 'Optimize postgres'
vacuumdb -h localhost -U postgres --all --analyze
