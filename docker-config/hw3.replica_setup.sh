#!/bin/sh

set -e
MASTER_CONTAINER="socialnetwork-db-master"

echo "Clearing existing data on replica"
rm -rf /var/lib/postgresql/data/*

echo "Copy backup to the replica"
PGPASSWORD=!QAZ2wsx pg_basebackup -h $MASTER_CONTAINER -D /var/lib/postgresql/data -U replicator -v -P --wal-method=stream
# cp -r /master_backup/* /var/lib/postgresql/data/

CONFIG_FILE="/var/lib/postgresql/data/postgresql.conf"
PRIMARY_CONNINFO="primary_conninfo = 'host=socialnetwork-db-master port=5432 user=replicator password=!QAZ2wsx'"
LAST_LINE=$(tail -n 1 "$CONFIG_FILE")
if [ "$LAST_LINE" != "$PRIMARY_CONNINFO" ]; then
  echo "Setup postgresql.conf"
  echo "Delete 3 last row from master"
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
  echo "Add primary_conninfo in postgresql.conf"
  # echo "primary_conninfo = 'host=$MASTER_CONTAINER port=5432 user=replicator password=!QAZ2wsx'" >> /var/lib/postgresql/data/postgresql.conf
  echo "$PRIMARY_CONNINFO" >> "$CONFIG_FILE"
else
  echo "Already exist primary_conninfo"
fi

if [ -f /var/lib/postgresql/data/standby.signal ]; then
  echo "Already exist standby.signal"
else
  echo "Create file standby.signal"
  touch /var/lib/postgresql/data/standby.signal
fi

echo "Clear WAL"
rm -rf /var/lib/postgresql/data/pg_wal/*

echo "Replication setup complete."