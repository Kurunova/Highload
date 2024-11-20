#!/bin/bash
set -e
POSTGRES_DATA="/var/lib/postgresql/data"
POSTGRES_DB_NAME="socialnetwork"
POSTGRES_USER="postgres_user"
POSTGRES_USER_REPLICATOR="replicator"
POSTGRES_PASSWORD="!QAZ2wsx"

echo "Start setup Master"

echo "Creating new postgresql.conf"
echo "# PostgreSQL configuration file
listen_addresses = '*'
wal_level = replica
max_wal_senders = 10
wal_keep_size = 64
archive_mode = on
archive_command = 'cd .'
hba_file = '$POSTGRES_DATA/pg_hba.conf'
" > $POSTGRES_DATA/postgresql.conf

echo "Creating new pg_hba.conf"
echo "# PostgreSQL client authentication configuration file
host    replication     $POSTGRES_USER_REPLICATOR      0.0.0.0/0               md5
host    all             all             0.0.0.0/0               md5
local   all             $POSTGRES_USER                                trust
" > $POSTGRES_DATA/pg_hba.conf

echo "Checking if user 'replicator' exists"
USER_EXISTS=$(psql -U $POSTGRES_USER -d $POSTGRES_DB_NAME -tAc "SELECT 1 FROM pg_roles WHERE rolname='$POSTGRES_USER_REPLICATOR'")

if [ "$USER_EXISTS" != "1" ]; then
  echo "User '$POSTGRES_USER_REPLICATOR' does not exist, creating user."
  psql -U $POSTGRES_USER -d $POSTGRES_DB_NAME -c "CREATE ROLE $POSTGRES_USER_REPLICATOR WITH REPLICATION PASSWORD '$POSTGRES_PASSWORD' LOGIN;"
else
  echo "User '$POSTGRES_USER_REPLICATOR' already exists, skipping creation."
fi

echo "End setup Master"