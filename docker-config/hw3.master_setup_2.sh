#!/bin/bash

echo "Waiting for master to be ready"
until pg_isready -U replicator -d socialnetwork; do
  echo "Waiting..."
  sleep 1
done
echo "Master is ready"

#TEMP_BACKUP_DIR="/var/lib/postgresql/master_backup_tmp"
#mkdir -p "$TEMP_BACKUP_DIR"
#chown postgres:postgres "$TEMP_BACKUP_DIR"

if [ -z "$(ls -A /master_backup)" ]; then
  echo "Backup directory is empty."  
else
  echo "Backup directory is not empty."
  echo "Clearing backup directory"
  rm -rf /master_backup/*
fi

echo "Create backup"
pg_basebackup -D /master_backup -U replicator -v -P --wal-method=stream

#echo "Copy backup to the master_backup"
#cp -r "$TEMP_BACKUP_DIR"/* /master_backup/

echo "Master backup complete."