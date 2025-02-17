## Запускаем окружение

```shell
docker-compose -f hw9.docker-compose.yml up -d --build --force-recreate
```

## Контейнер для тестов test-runner

Войти в контейнер

```shell
docker exec -it test-runner bash
docker exec -it test-runner sh
```

Если pgbench не установлен внутри контейнера, его можно установить:

```shell
apk add --no-cache postgresql-client
```

Если ab не установлен внутри контейнера, его можно установить:

```shell
apk add --no-cache apache2-utils
```

## Проверяем работу HAProxy

### Нагрузка на DB

```shell
docker exec -it test-runner sh -c 'PGPASSWORD="!QAZ2wsx" pgbench -h haproxy -p 5432 -U postgres_user -d socialnetwork -c 10 -T 60 -f /test.sql'
docker exec -it test-runner sh -c 'PGPASSWORD="!QAZ2wsx" pgbench -h haproxy -p 5432 -U postgres_user -d socialnetwork -c 10 -T 60 -f /test_insert.sql'
docker exec -it test-runner sh -c 'PGPASSWORD="!QAZ2wsx" pgbench -h haproxy -p 5432 -U postgres_user -d socialnetwork -c 10 -T 60 -f /test_update.sql'
```

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
ab -n 1000 -c 10 http://localhost:8080/
docker exec -it test-runner ab -n 1000 -c 10 http://nginx/

wrk -t4 -c10 -d60s http://localhost:8080/
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