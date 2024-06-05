Social Network 
============

# Getting Started

## Environment

Start all containers:

```
docker-compose up -d --build --force-recreate
```

## Setup JMeter

### JMeter additional Graph plugins

Install JMeter PluginManager
Download and put into ext/lib folder
Open PluginManager and install 3 Basic Graph

### JMeter Graph with Grafana and InfluxDB

#### Setup InfluxDB

Go to the page http://localhost:8086/
Fill in next value
INFLUXDB_ORGANIZATION_NAME=socialnetwork
INFLUXDB_BUCKET_NAME=loadtest

#### JMeter Backend Listener

Add Backend Listener 
Select InfluxdbBackendListenerClient (install additional plugin)
Url: http://localhost:8086/write?db=LoadTestDb
Url: http://localhost:8086/api/v2/write?org=socialnetwork&bucket=loadtest
influxdbToken: from ui

#### Grafana Dashboard

1. Add DataSource
URL: http://influxdb:8086
Basic auth: admin
Password: adminpassword
For Flux use:
Organization: socialnetwork
Token: from UI http://localhost:8086/ get API token
Default Bucket: loadtest

2. Add Dashboard
Download json here: https://grafana.com/grafana/dashboards/5496-apache-jmeter-dashboard-by-ubikloadpack/
and import in Grafana

## Setup metrics 

### Application container exporter / Prometheus / Grafana dashboard

container_cpu_usage_seconds_total{name="socialnetwork-1"} 
container_memory_usage_bytes{name="socialnetwork-1"}
container_fs_io_time_seconds_total{name="socialnetwork-1"}

### PostgreSql exporter / Prometheus / Grafana dashboard

#### Текущее количество активных соединений
pg_stat_activity_count{datname="socialnetwork"}
pg_stat_activity_count{datname="socialnetwork", state="active"}
#### Индексные и последовательные сканы по таблицам
Последовательные сканы
pg_stat_user_tables_seq_scan{datname="socialnetwork"}
Индексные сканы
pg_stat_user_tables_idx_scan{datname="socialnetwork"}
#### Статистика фонового записывающего процесса
Сбросы страниц
pg_stat_bgwriter_buffers_checkpoint{datname="socialnetwork"}
Очистки страниц
pg_stat_bgwriter_buffers_clean{datname="socialnetwork"}
#### Общая статистика по базам данных
Количество транзакций
pg_stat_database_xact_commit{datname="socialnetwork"}
Количество операций чтения
pg_stat_database_blks_read{datname="socialnetwork"}
####  Информация о блокировках
pg_locks_count{datname="socialnetwork"}
#### Статистика репликации
Задержка репликации в байтах
pg_stat_replication_delay_bytes{datname="socialnetwork"}
Состояние репликации
pg_stat_replication_state{datname="socialnetwork"}
#### Размер базы данных
pg_database_size_bytes{datname="socialnetwork"}
#### Размер табличного пространства
pg_tablespace_size_bytes{datname="socialnetwork", tablespacename="your_tablespace_name"}
#### Максимальная длительность транзакций
pg_stat_activity_max_tx_duration_seconds{datname="socialnetwork"}
