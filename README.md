Social Network 
============

# Getting Started

## Environment

Start all containers:

```
docker-compose up -d --build --force-recreate
```

Application API: http://localhost:5001/swagger/index.html
Connect to DB: 
    - localhost:5400
    - postgres_user:!QAZ2wsx
Grafana: http://localhost:3000/
    - admin:!QAZ2wsx
Prometheus: http://localhost:9090/, check exporters statuses http://localhost:9090/targets
cadvisor: http://localhost:8080/
Postgres exporter: http://localhost:9190/

## Migrations

Manual run up migrations
```
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator"
```
Manual run down migrations
```
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.RollbackAll"
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.RollbackOne"
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.Rollback"
```

## Setup JMeter

### JMeter additional Graph plugins

Install JMeter PluginManager
Download and put into ext/lib folder
Open PluginManager and install 3 Basic Graph

## Setup Grafana Dashboard

1. Add DataSource
Select Prometheus
URL: http://prometheus:9090/

2. Add Dashboard
Import a dashboard from folder grafana_dashboards (SocialNetwork Api-1729679954881.json, )
or create new Dashboard

Create new Dashboard
- Choose DataSource as Prometheus

Dashboard: SocialNetwork Api
1. Visualisation: CPU
   - group by(cpu) (container_cpu_usage_seconds_total{name="socialnetwork-api-1", job="cadvisor"})
2. Visualisation: Memory
   - container_memory_usage_bytes{name="socialnetwork-api-1", job="cadvisor"}
3. Visualisation: IO
   - container_fs_usage_bytes{name="socialnetwork-api-1", job="cadvisor"}
   - container_fs_reads_total{name="socialnetwork-api-1", job="cadvisor"}
   - container_fs_writes_total{name="socialnetwork-api-1", job="cadvisor"}
4. Visualisation: Network
   - container_network_receive_bytes_total{name="socialnetwork-api-1", job="cadvisor"}
   - container_network_transmit_bytes_total{name="socialnetwork-api-1", job="cadvisor"}
5. Visualisation: File System inodes
   - container_fs_inodes_total{name="socialnetwork-api-1", job="cadvisor"}
   - container_fs_inodes_free{name="socialnetwork-api-1", job="cadvisor"}

Dashboard: SocialNetwork Db Master
1. Visualisation: Текущее количество активных соединений
   - pg_stat_activity_count{datname="socialnetwork", state="active"}
   - legend {{datname}}
2. Visualisation: Индексные и последовательные сканы по таблицам
   - Последовательные сканы
   - pg_stat_user_tables_seq_scan{datname="socialnetwork", relname="users"}
   - legend tables_seq_{{relname}}
   - Индексные сканы
   - pg_stat_user_tables_idx_scan{datname="socialnetwork", relname="users"}
   - legend tables_idx_{{relname}}
3. Visualisation: Общая статистика по базам данных
   - Количество транзакций
   - pg_stat_database_xact_commit{datname="socialnetwork"}
   - Количество операций чтения
   - pg_stat_database_blks_read{datname="socialnetwork"}
4. Visualisation: Информация о блокировках
   - pg_locks_count{datname="socialnetwork"}
   - legend {{mode}}
5. Visualisation: Статистика репликации
   - Задержка репликации в байтах
   - pg_stat_replication_delay_bytes{datname="socialnetwork"}
   - Состояние репликации
   - pg_stat_replication_state{datname="socialnetwork"}
6. Visualisation: Размер базы данных
   - pg_database_size_bytes{datname="socialnetwork"}
   - legend {{datname}}
7. Visualisation: Размер табличного пространства
   - pg_tablespace_size_bytes{datname="socialnetwork", tablespacename="your_tablespace_name"}
8. Visualisation: Максимальная длительность транзакций
   - pg_stat_activity_max_tx_duration_seconds{datname="socialnetwork"}

