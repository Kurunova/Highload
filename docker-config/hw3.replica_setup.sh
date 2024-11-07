#!/bin/sh

set -e
MASTER_CONTAINER="socialnetwork-db-master"

if [ -f /var/lib/postgresql/data/.replica_setup_1 ]; then
  echo "Initialization already done for replica, skipping."
else
  echo "Running initialization script for the first time on replica..."

  echo "Copy backup to the replica"
  cp -r /master_backup/* /var/lib/postgresql/data/
  
  echo "Create file standby.signal"
  touch /var/lib/postgresql/data/standby.signal
  
  echo "Setup postgresql.conf"
  echo "Delete 3 last row from master"
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  echo "Add primary_conninfo"
  echo "primary_conninfo = 'host=$MASTER_CONTAINER port=5432 user=replicator password=!QAZ2wsx'" >> /var/lib/postgresql/data/postgresql.conf

  echo "Create flag-file"
  touch /var/lib/postgresql/data/.replica_setup_1
fi

echo "Replication setup complete."