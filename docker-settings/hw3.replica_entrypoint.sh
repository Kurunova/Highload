#!/bin/bash

echo "Starting replica setup..."

set -e
MASTER_CONTAINER="socialnetwork-db-master"
POSTGRES_DATA="/var/lib/postgresql/data"
POSTGRES_USER="replicator"
POSTGRES_PASSWORD="!QAZ2wsx"

wait_for_master() {
  until pg_isready -h $MASTER_CONTAINER -U "$POSTGRES_USER"; do
    echo "Waiting for master to be ready..."
    sleep 2
  done
}

# Wait until the master server is ready to accept connections
wait_for_master

# Set correct permissions on data directory
chown -R postgres:postgres $POSTGRES_DATA
chmod 700 $POSTGRES_DATA

#echo "Clearing existing data on replica"
#rm -rf $POSTGRES_DATA/*
#
## Wait until data directory is fully cleared
#while [ "$(ls -A $POSTGRES_DATA 2>/dev/null)" ]; do
#  echo "Waiting for data directory to be fully cleared..."
#  sleep 1
#done

if [ "$(ls -A $POSTGRES_DATA)" ]; then
   echo "Data directory is not empty. Skipping base backup."
else
   echo "Data directory is empty. Performing base backup from master."
   PGPASSWORD=$POSTGRES_PASSWORD pg_basebackup -h $MASTER_CONTAINER -D $POSTGRES_DATA -U $POSTGRES_USER -v -P --wal-method=stream
fi

if [ -f $POSTGRES_DATA/standby.signal ]; then
  echo "Skipping, already exist standby.signal"
else
  # Add replication settings to postgresql.conf
  echo "primary_conninfo = 'host=$MASTER_CONTAINER port=5432 user=$POSTGRES_USER password=$POSTGRES_PASSWORD'" >> $POSTGRES_DATA/postgresql.conf
  
  # Create a standby.signal file to enable standby mode
  touch $POSTGRES_DATA/standby.signal
fi

# Start the PostgreSQL server
exec "$@"