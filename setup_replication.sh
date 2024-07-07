#!/bin/sh

set -e
MASTER_CONTAINER="socialnetwork-db-master"

# Wait for the master to be ready
until pg_isready -h $MASTER_CONTAINER -U test -d socialnetwork; do
  echo "Waiting for master to be ready..."
  sleep 1
done

# Ensure the replicator user exists
PGPASSWORD=test psql -h $MASTER_CONTAINER -U test -d socialnetwork -c "
DO LANGUAGE plpgsql \$\$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'replicator') THEN
    PERFORM pg_sleep(1);
    CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD 'pass';
  END IF;
END
\$\$;"

echo "Current pg_hba.conf on master:"
PGPASSWORD=test psql -h $MASTER_CONTAINER -U test -d socialnetwork -c "SELECT pg_read_file('pg_hba.conf');"

# Create a backup from the master
PGPASSWORD=test pg_basebackup -h $MASTER_CONTAINER -p 5432 -D /master_data -U test -v -P --wal-method=stream

# Copy the backup to the replicas
cp -r /master_data/* /replica1_data/
cp -r /master_data/* /replica2_data/

# Setup the first replica
if [ -f /replica1_data/.init_done ]; then
  echo "Initialization already done for replica1, skipping."
else
  echo "Running initialization script for the first time on replica1..."
  touch /replica1_data/standby.signal
  echo "primary_conninfo = 'host=$MASTER_CONTAINER port=5432 user=replicator password=pass'" >> /replica1_data/postgresql.conf
  touch /replica1_data/.init_done
fi

# Setup the second replica
if [ -f /replica2_data/.init_done ]; then
  echo "Initialization already done for replica2, skipping."
else
  echo "Running initialization script for the first time on replica2..."
  touch /replica2_data/standby.signal
  echo "primary_conninfo = 'host=$MASTER_CONTAINER port=5432 user=replicator password=pass'" >> /replica2_data/postgresql.conf
  touch /replica2_data/.init_done
fi

# Restart PostgreSQL on the replicas
pg_ctl -D /replica1_data restart
pg_ctl -D /replica2_data restart

echo "Replication setup complete."