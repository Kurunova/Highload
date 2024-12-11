# проверить конфиг в бд
# docker exec -it socialnetwork-db-master psql -U test -d socialnetwork -c "SELECT pg_read_file('pg_hba.conf');"
# SELECT pg_read_file('pg_hba.conf');

# проверить конфиг физически
# docker exec -it socialnetwork-db-master cat /var/lib/postgresql/data/pg_hba.conf

# перезапуск контейнера после изменений
# docker restart socialnetwork-db-master

# пути к файлам 
# SHOW config_file;
# SHOW hba_file;
# SHOW data_directory;

# посмотреть содержимое
# cat /var/lib/postgresql/data/postgresql.conf
# cat /var/lib/postgresql/data/pg_hba.conf
# cat /var/lib/postgresql/data/standby.signal

# ls /var/lib/postgresql/data/
# touch /var/lib/postgresql/data/standby.signal
# cat /var/lib/postgresql/data/postgresql.conf
# head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf
# echo "primary_conninfo = 'host=socialnetwork-db-master port=5432 user=replicator password=!QAZ2wsx'" >> /var/lib/postgresql/data/postgresql.conf
# echo "primary_conninfo = 'host=socialnetwork-db-replica-1 port=5432 user=replicator password=!QAZ2wsx'" >> /var/lib/postgresql/data/postgresql.conf
 
 # echo "primary_conninfo = 'host=socialnetwork-db-replica-1 port=5432 user=replicator password=!QAZ2wsx'" >> /var/lib/postgresql/data/pg_hba.conf
 
# удалить строку последнюю
# head -n -1 /var/lib/postgresql/data/pg_hba.conf > /var/lib/postgresql/data/pg_hba.conf.tmp && mv /var/lib/postgresql/data/pg_hba.conf.tmp /var/lib/postgresql/data/pg_hba.conf
# head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf

# добавить настройку логов в postgresql
#  echo "log_destination = 'stderr'" >> /var/lib/postgresql/data/postgresql.conf
#  echo "logging_collector = on" >> /var/lib/postgresql/data/postgresql.conf
#  echo "log_min_messages = debug1" >> /var/lib/postgresql/data/postgresql.conf

#set -e
#MASTER_CONTAINER="socialnetwork-db-master"

# Wait for the master to be ready
#until pg_isready -h $MASTER_CONTAINER -U test -d socialnetwork; do
#  echo "Waiting for master to be ready..."
#  sleep 1
#done

# Ensure the replicator user exists
#PGPASSWORD=test psql -h $MASTER_CONTAINER -U test -d socialnetwork -c "
#DO LANGUAGE plpgsql \$\$
#BEGIN
#  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'replicator') THEN
#    PERFORM pg_sleep(1);
#    CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD 'pass';
#  END IF;
#END
#\$\$;"

#echo "Current pg_hba.conf on master:"
#PGPASSWORD=test psql -h $MASTER_CONTAINER -U test -d socialnetwork -c "SELECT pg_read_file('pg_hba.conf');"

#mkdir -p /replica1_data/log
#chown postgres:postgres /replica1_data/log
#mkdir -p /replica2_data/log
#chown postgres:postgres /replica2_data/log

#echo "Create backup"
#if [ -z "$(ls -A /master_backup)" ]; then
#  echo "Backup directory is empty, creating a new backup..."
#  PGPASSWORD=test pg_basebackup -h $MASTER_CONTAINER -p 5432 -D /master_backup -U test -v -P --wal-method=stream
#  
#  # Copy the backup to the replicas
#  cp -r /master_backup/* /replica1_data/  
#  cp -r /master_backup/* /replica2_data/
#else
#  echo "Backup directory is not empty, skipping backup creation."
#fi

#echo "Restart PostgreSQL after changes"
#pg_ctl -D /var/lib/postgresql/data restart
#
#echo "Waiting for replica to be ready"
#until pg_isready -U replicator -d socialnetwork; do
#  echo "Waiting..."
#  sleep 1
#done
#echo "Replica is ready"

#if [ -f /var/lib/postgresql/data/.replica_setup_1 ]; then
#  echo "Initialization already done for replica, skipping."
#else
#  echo "Running initialization script for the first time on replica..."
#  
#
#  echo "Create flag-file"
#  touch /var/lib/postgresql/data/.replica_setup_1
#fi

#psql -U postgres_user -d socialnetwork -c "
##DO LANGUAGE plpgsql \$\$
##BEGIN
##  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'replicator') THEN
##    PERFORM pg_sleep(1);
##    CREATE ROLE replicator WITH LOGIN REPLICATION PASSWORD '!QAZ2wsx';
##  END IF;
##END
##\$\$;"

#echo "Restart PostgreSQL after changes"
#pg_ctl -D /var/lib/postgresql/data restart

#echo "Waiting for master to be ready"
#until pg_isready -U replicator -d socialnetwork; do
#  echo "Waiting..."
#  sleep 1
#done
#echo "Master is ready"

#echo "Create backup"
#if [ -z "$(ls -A /master_backup)" ]; then
#  echo "Backup directory is empty, creating a new backup..."
#  pg_basebackup -D /master_backup -U replicator -v -P --wal-method=stream
#else
#  echo "Backup directory is not empty, skipping backup creation."
#fi

#echo "Waiting for master to be ready"
#until pg_isready -h $MASTER_CONTAINER -U replicator -d socialnetwork; do
#  echo "Waiting..."
#  sleep 1
#done
#echo "Master is ready"
#
#echo "Create backup"
#if [ -z "$(ls -A /master_backup)" ]; then
#  echo "Backup directory is empty, creating a new backup..."
#  PGPASSWORD=!QAZ2wsx pg_basebackup -h $MASTER_CONTAINER -p 5432 -D /master_backup -U replicator -v -P --wal-method=stream
#else
#  echo "Backup directory is not empty, skipping backup creation."
#fi
#
#echo "Master backup complete."