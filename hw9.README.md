## Запускаем окружение

```shell
docker-compose -f hw9.docker-compose.yml up -d --build --force-recreate
```

Connect to DB:
- localhost:5400
- postgres_user:!QAZ2wsx
Grafana: http://localhost:3000/
- admin:!QAZ2wsx
Haproxy stats: http://localhost:8404/stats

Проверить что бд доступна через DBeaver, подключиться к порту 5410 и выполнить
```sql
select count(*)
FROM public.users
order by 1 desc;
```

## Контейнер для тестов test-runner

Войти в контейнер
```shell
docker exec -it test-runner bash
```

Если pgbench не установлен внутри контейнера, его можно установить:
```shell
apk add --no-cache postgresql-client
```

Если ab не установлен внутри контейнера, его можно установить:
```shell[Dockerfile.TestRunner](docker-settings%2FDockerfile.TestRunner)
apk add --no-cache apache2-utils
```

## Проверяем работу HAProxy

### Нагрузка на DB

Запустить тест

```shell
docker exec -it test-runner sh -c 'PGPASSWORD="!QAZ2wsx" pgbench -h haproxy -p 5432 -U postgres_user -d socialnetwork -c 50 -j 4 -T 60 -f /test.sql'
```
-c - количество одновременных соединений
-T - время выполнения теста

### Тестирование DB

Отключение одной реплики PostgreSQL
```shell
docker stop socialnetwork-db-replica-1
```

Проверяем логи
```shell
docker logs haproxy
```

Запускаем реплику обратно
```shell
docker start socialnetwork-db-replica-1
```

## Проверяем работу Nginx

### Нагрузка на API

Можно использовать ab (Apache Benchmark) или wrk для генерации запросов:
```shell
docker exec -it test-runner ab -n 1000 -c 10 http://nginx/
docker exec -it test-runner wrk -t10 -c10 -d60s http://nginx/
```

### Тестирование API

Отключение одного API-инстанса
```shell
docker stop socialnetwork-api-1
```

Проверяем логи Nginx
```shell
docker logs nginx
```

Возвращаем API обратно
```shell
docker start socialnetwork-api-1
```

-----------
Для отладки
-----------

Создаем временный контейнер
```shell
docker run --rm -it --network otushighloadarchitect_sn-network alpine sh
docker run --rm -it --network otushighloadarchitect_sn-network postgres:latest sh
```

Установить
```shell
apk add --no-cache netcat-openbsd postgresql-client busybox-extras
```

Подключаемся к PostgreSQL через HAProxy
```shell
docker run --rm -it -e PGPASSWORD="!QAZ2wsx" --network otushighloadarchitect_sn-network postgres:latest psql -h haproxy -p 5432 -U postgres_user -d socialnetwork -c "SELECT NOW();"
```