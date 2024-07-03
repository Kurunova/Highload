#!/bin/bash

# Проверка наличия флаг-файла
if [ -f /var/lib/postgresql/data/.init_done ]; then
  echo "Initialization already done, skipping."
else
  echo "Running initialization script for the first time..."

  # Настройки для репликации
  echo "ssl = off" >> /var/lib/postgresql/data/postgresql.conf
  echo "wal_level = replica" >> /var/lib/postgresql/data/postgresql.conf
  echo "max_wal_senders = 2" >> /var/lib/postgresql/data/postgresql.conf

  # Настройка pg_hba.conf для репликации
  echo "host replication replicator 0.0.0.0/0 md5" >> /var/lib/postgresql/data/pg_hba.conf

  # Создание флаг-файла
  touch /var/lib/postgresql/data/.init_done
fi

# Запуск PostgreSQL
# exec postgres