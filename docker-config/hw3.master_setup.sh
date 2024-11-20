#!/bin/bash

echo "Start setup Master"

echo "Creating new postgresql.conf"
echo "# PostgreSQL configuration file
listen_addresses = '*'
wal_level = replica
max_wal_senders = 10
wal_keep_size = 64
archive_mode = on
archive_command = 'cd .'
hba_file = '/var/lib/postgresql/data/pg_hba.conf'
" > /var/lib/postgresql/data/postgresql.conf

echo "Creating new pg_hba.conf"
echo "# PostgreSQL client authentication configuration file
host    replication     replicator      0.0.0.0/0               md5
host    all             all             0.0.0.0/0               md5
local   all             postgres_user                                trust
" > /var/lib/postgresql/data/pg_hba.conf

echo "Checking if user 'replicator' exists"
USER_EXISTS=$(psql -U postgres_user -d socialnetwork -tAc "SELECT 1 FROM pg_roles WHERE rolname='replicator'")

if [ "$USER_EXISTS" != "1" ]; then
  echo "User 'replicator' does not exist, creating user."
  psql -U postgres_user -d socialnetwork -c "CREATE ROLE replicator WITH REPLICATION PASSWORD '!QAZ2wsx' LOGIN;"
else
  echo "User 'replicator' already exists, skipping creation."
fi

#echo "Check existing of flag-file master_setup_1"
#if [ -f /var/lib/postgresql/data/.master_setup_1 ]; then
#  echo "Initialization already done, skipping."
#else 
#  echo "Setup postgresql.conf"
#  # echo "ssl = off" >> /var/lib/postgresql/data/postgresql.conf
#  echo "wal_level = replica" >> /var/lib/postgresql/data/postgresql.conf
#  echo "max_wal_senders = 4" >> /var/lib/postgresql/data/postgresql.conf
#  echo "listen_addresses = '*'" >> /var/lib/postgresql/data/postgresql.conf
#  # echo "hba_file = '/var/lib/postgresql/data/pg_hba.conf'" >> /var/lib/postgresql/data/postgresql.conf
#
#  echo "Setup pg_hba.conf"
#  echo "host    replication     replicator      0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf
#  echo "host    all             all             0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf
#
#  echo "Add user replicator"
#  psql -U postgres_user -d socialnetwork -c "CREATE ROLE replicator WITH REPLICATION PASSWORD '!QAZ2wsx' LOGIN;"
#
#  echo "Create flag-file"
#  touch /var/lib/postgresql/data/.master_setup_1
#fi

echo "End setup Master"