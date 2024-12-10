Social Network 
============

# Getting Started

## Report

====================
Часть 1
====================

Добавлены в docker-compose: citus-coordinator, citus-worker-1, citus-worker-2
Новая строка подключения: DialogDbSettings__MasterConnectionString 

Запускаем с новыми настройками
```shell
docker-compose -f hw5.1.docker-compose.yml up -d --build --force-recreate
```

Подключаемся к координатору (например, DBeaver - localhost:5500) и регистрируем воркеры
```sql
SELECT master_add_node('citus-worker-1', 5432);
SELECT master_add_node('citus-worker-2', 5432);
```

Проверяем подключение воркеров
```sql
SELECT * FROM master_get_active_worker_nodes();
```

Создаем таблицу в citus координаторе
```sql
CREATE TABLE dialog_messages (
     id BIGSERIAL NOT NULL,
     dialog_id text NOT NULL,
     from_user_id BIGINT NOT NULL,
     to_user_id BIGINT NOT NULL,
     text TEXT NOT NULL,
     sent_at TIMESTAMP WITHOUT TIME ZONE NOT NULL,
     PRIMARY KEY (dialog_id, id)
);
```

Настраиваем шардирование по ключу
```sql
SELECT create_distributed_table('dialog_messages', 'dialog_id');
```

Проверяем количество шардов
```sql
SELECT shard_count FROM citus_tables WHERE table_name::text = 'dialog_messages';
```

Проверяем, что данные равномерно распределились по шардам:
```sql
SELECT nodename, count(*)
FROM citus_shards GROUP BY nodename;
```

```shell
INSERT INTO dialog_messages (dialog_id, from_user_id, to_user_id, text, sent_at)
VALUES ('1000_1005257', 1000, 1005257, 'Hello', '2024-12-10 12:00:00');

select *
from dialog_messages
limit 1000;

explain SELECT *
FROM citus.dialog_messages
WHERE dialog_id = '1000_1005257'
ORDER BY sent_at;

explain select *
from dialog_messages
limit 1000;
```

====================
Часть 2
====================

Добавлены в docker-compose: citus-worker-3

Запускаем с новыми настройками
```shell
docker-compose -f hw5.2.docker-compose.yml up -d --build --force-recreate
```

```shell
docker cp ./docker-settings/citus-coordinator/.pgpass citus-coordinator:/var/lib/postgresql/.pgpass
docker-compose -f hw5.2.docker-compose.yml up -d citus-coordinator
```


Регистрируем воркера
```sql
SELECT master_add_node('citus-worker-3', 5432);
```

Смотрим количество шардов
```sql
SELECT shard_count FROM citus_tables WHERE table_name::text = 'dialog_messages';
```

Проверяем распределение данных по шардам
```sql
SELECT nodename, count(*)
FROM citus_shards GROUP BY nodename;
```

Ребалансировка после добавления новых воркеров
```sql
SELECT rebalance_table_shards('dialog_messages');
SELECT citus_rebalance_start();
```

Следим за статусом ребалансировки
```sql
SELECT * FROM citus_rebalance_status();
```

Установка wal_level = logical чтобы узлы могли переносить данные:
```sql
alter system set wal_level = logical;
SELECT run_command_on_workers('alter system set wal_level = logical');
```
Проверка wal_level
```sql
show wal_level;
```
     



## Environment

Start all containers:

```shell
docker-compose -f hw5.1.docker-compose.yml up -d --build --force-recreate
docker-compose -f hw5.1.docker-compose.yml down -v

docker-compose -f hw5.2.docker-compose.yml up -d --build --force-recreate
docker-compose -f hw5.2.docker-compose.yml down -v
```

Application API: http://localhost:5001/swagger/index.html
Авторизация Bearer {token}
Connect to DB: 
    - localhost:5400
    - postgres_user:!QAZ2wsx
Grafana: http://localhost:3000/
    - admin:!QAZ2wsx
Prometheus: http://localhost:9090/, check exporters statuses http://localhost:9090/targets
cadvisor: http://localhost:8080/
Postgres exporter: http://localhost:9190/
Redis UI: http://localhost:8081/
Redis exporter: http://localhost:9121/


## Migrations

Manual run up migrations
```shell
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator"
```
Manual run down migrations
```shell
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.RollbackAll"
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.RollbackOne"
dotnet run --project src/SocialNetwork.DbMigrator --launch-profile "SocialNetwork.DbMigrator.Rollback"
```


----------
old
----------


Обновляем настройки
```shell
docker cp ./docker-settings/citus-coordinator/pg_hba.conf citus-coordinator:/var/lib/postgresql/data/pg_hba.conf
docker cp ./docker-settings/citus-coordinator/.pgpass citus-coordinator:/var/lib/postgresql/.pgpass
docker cp ./docker-settings/citus-worker/pg_hba.conf citus-worker-1:/var/lib/postgresql/data/pg_hba.conf
docker cp ./docker-settings/citus-worker/pg_hba.conf citus-worker-2:/var/lib/postgresql/data/pg_hba.conf
```

Рестарт с новыми настройками
```shell
docker-compose -f hw5.docker-compose.yml up -d --build --force-recreate
```

Проверяем настройки
```shell
docker exec -it citus-coordinator cat /var/lib/postgresql/data/pg_hba.conf
docker exec -it citus-coordinator cat  /var/lib/postgresql/.pgpass
docker exec -it citus-worker-1 cat /var/lib/postgresql/data/pg_hba.conf
docker exec -it citus-worker-2 cat /var/lib/postgresql/data/pg_hba.conf
```


Перенос данных из старой таблицы
```shell
docker exec -it socialnetwork-db-master pg_dump -U postgres_user -d socialnetwork -t dialog_messages --data-only --column-inserts > dialog_messages_data.sql
```
Импорт данных в шардированную таблицу
```shell
docker exec -i citus-coordinator psql -U postgres_user -d socialnetwork < dialog_messages_data.sql
Get-Content dialog_messages_data.sql | docker exec -i citus-coordinator psql -U postgres_user -d socialnetwork
```