#!/bin/bash

until pg_isready -h socialnetwork-db-master; do
  echo "Waiting for master to be ready..."
  sleep 1
done

# Создание пользователя для репликации на мастере
PGPASSWORD=test psql -h socialnetwork-db-master -U test -d socialnetwork -c "DO \$\$ BEGIN
  IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'replicator') THEN
    CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD 'pass';
  END IF;
END \$\$;"

# Проверка наличия флаг-файла
if [ -f /var/lib/postgresql/data/.init_done ]; then
  echo "Initialization already done, skipping."
else
  echo "Running initialization script for the first time..."

  chown -R postgres:postgres /var/lib/postgresql/data
  chmod 700 /var/lib/postgresql/data

  pg_basebackup -h socialnetwork-db-master -D /var/lib/postgresql/data -U replicator -Fp -Xs -P -R
  echo "primary_conninfo = 'host=socialnetwork-db-master port=5432 user=replicator password=pass'" >> /var/lib/postgresql/data/postgresql.conf
  echo "ssl = off" >> /var/lib/postgresql/data/postgresql.conf
  
  touch /var/lib/postgresql/data/standby.signal

  # Создание флаг-файла
  touch /var/lib/postgresql/data/.init_done
fi

# exec postgres