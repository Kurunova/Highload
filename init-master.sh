#!/bin/bash

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

# удалить строку последнюю
# head -n -1 /var/lib/postgresql/data/pg_hba.conf > /var/lib/postgresql/data/pg_hba.conf.tmp && mv /var/lib/postgresql/data/pg_hba.conf.tmp /var/lib/postgresql/data/pg_hba.conf
# head -n -1 /var/lib/postgresql/data/postgresql.conf > /var/lib/postgresql/data/postgresql.conf.tmp && mv /var/lib/postgresql/data/postgresql.conf.tmp /var/lib/postgresql/data/postgresql.conf

# посмотреть содержимое
# cat /var/lib/postgresql/data/postgresql.conf
# cat /var/lib/postgresql/data/pg_hba.conf

# проверка наличия флаг-файла
if [ -f /var/lib/postgresql/data/.init_done ]; then
  echo "Initialization already done, skipping."
else
  echo "Running initialization script for the first time..."

  # логи
  echo "log_destination = 'stderr'" >> /var/lib/postgresql/data/postgresql.conf
  echo "logging_collector = on" >> /var/lib/postgresql/data/postgresql.conf
  echo "log_min_messages = debug1" >> /var/lib/postgresql/data/postgresql.conf

  # Настройки для репликации  
  # echo "ssl = off" >> /var/lib/postgresql/data/postgresql.conf
  echo "wal_level = replica" >> /var/lib/postgresql/data/postgresql.conf
  echo "max_wal_senders = 4" >> /var/lib/postgresql/data/postgresql.conf
  echo "listen_addresses = '*'" >> /var/lib/postgresql/data/postgresql.conf
  # echo "hba_file = '/var/lib/postgresql/data/pg_hba.conf'" >> /var/lib/postgresql/data/postgresql.conf

  # настройка pg_hba.conf для репликации
  echo "host    replication     replicator      0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf
  echo "host    replication     test      0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf
  # чтобы можно было подключиться с бд клиента
  echo "host    all             all             0.0.0.0/0               md5" >> /var/lib/postgresql/data/pg_hba.conf

  # создание флаг-файла
  touch /var/lib/postgresql/data/.init_done
fi