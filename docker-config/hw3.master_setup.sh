#!/bin/bash

echo "Check existing of flag-file master_setup_1"
if [ -f /var/lib/postgresql/data/.master_setup_1 ]; then
  echo "Initialization already done, skipping."
else
  echo "Setup postgresql.conf"
  # echo "ssl = off" >> /var/lib/postgresql/data/postgresql.conf
  echo "wal_level = replica" >> /var/lib/postgresql/data/postgresql.conf
  echo "max_wal_senders = 4" >> /var/lib/postgresql/data/postgresql.conf
  echo "listen_addresses = '*'" >> /var/lib/postgresql/data/postgresql.conf
  # echo "hba_file = '/var/lib/postgresql/data/pg_hba.conf'" >> /var/lib/postgresql/data/postgresql.conf

  echo "Setup pg_hba.conf"
  echo "host    replication     replicator      0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf
  echo "host    all             all             0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf

  echo "Add user replicator"
  psql -U postgres_user -d socialnetwork -c "CREATE ROLE replicator WITH REPLICATION PASSWORD '!QAZ2wsx' LOGIN;"

  echo "Create flag-file"
  touch /var/lib/postgresql/data/.master_setup_1
fi

echo "Master setup complete."