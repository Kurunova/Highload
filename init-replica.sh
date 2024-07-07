#!/bin/bash

#until pg_isready -h socialnetwork-db-master -U replicator; do
#  echo "Waiting for master to be ready..."
#  sleep 1
#done

# Создание пользователя для репликации на мастере
#PGPASSWORD=test psql -h socialnetwork-db-master -U test -d socialnetwork -c "DO \$\$ BEGIN
#  IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'replicator') THEN
#    CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD 'pass';
#  END IF;
#END \$\$;"

#chown -R postgres:postgres /var/lib/postgresql/data
#chmod 700 /var/lib/postgresql/data

#pg_basebackup -h socialnetwork-db-master -p 5432 -D /var/lib/postgresql/data -U replicator -v -P --wal-method=stream
#if [ $? -ne 0 ]; then
#  echo "pg_basebackup failed, exiting."
#  exit 1
#fi

# echo "host all all 0.0.0.0/0 md5" >> /var/lib/postgresql/data/pg_hba.conf

#rm -rf /var/lib/postgresql/data/* 

# делаем бекап
#pg_basebackup -h socialnetwork-db-master -p 5432 -D /tmp/backup -U replicator -v -P --wal-method=stream

# cp -r /tmp/backup /var/lib/postgresql/data

# Проверка наличия флаг-файла
#if [ -f /var/lib/postgresql/data/.init_done ]; then
#  echo "Initialization already done, skipping."
#else
#  echo "Running initialization script for the first time..."
#  
#  touch /var/lib/postgresql/data/standby.signal
#  
#  echo "primary_conninfo = 'host=socialnetwork-db-master port=5432 user=replicator password=pass'" >> /var/lib/postgresql/data/postgresql.conf
#
#  # Создание флаг-файла
#  touch /var/lib/postgresql/data/.init_done
#fi

# exec postgres