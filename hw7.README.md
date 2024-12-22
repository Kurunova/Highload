Запускаем контейнеры 

```shell
docker-compose -f hw7.docker-compose.yml up -d --build --force-recreate
docker-compose -f hw7.docker-compose.yml up -d --build --force-recreate tarantool-db
```

# https://www.tarantool.io/en/doc/2.11/how-to/getting_started_db/

Подключаемся к tarantool

```shell
docker exec -it tarantool-db console

# получаем спейс  
s = box.space.dialog_messages

# добавляем запись 
s:insert{1, '1000_1005257', 1000, 1005257, 'Hello !', '2024-12-21'}
s:insert{2, '1005259_1005261', 1005261, 1005259, 'Hello !', '2024-12-21'}
s:insert{3, '1005259_1005261', 1005261, 1005259, 'Hello 2!', '2024-12-21'}

box.space.dialog_messages:insert{2, '1000_1005257', 1000, 1005257, 'Hello !', '2024-12-21'}
add_dialog_message('1000_1005257', 1000, 1005257, 'Hello from function!', '2024-12-21')

# находим запись 
s:select{'1000_1005257'}
s:select{'1005259_1005261'}

box.space.dialog_messages:select{'1000_1005257'}
```

Режим terminal
```shell
docker exec -it tarantool-db tarantool
```


Логинимся под юзером 1 (1005261) ```/Users/login```
```json
{
    "login": "elenaf1",
        "password": "!QAZ2wsx"
}
```
Добавляем Bearer и отправляем сообщение юзеру 2 (1005259)
```/Dialogs/{user_id}/send```
```json
{
  "text": "First dialog message"
}
```

Логинимся под юзером 2 (1005259) ```/Users/login``` (режим инкогнито)
```json
{
    "login": "elenaf2",
    "password": "!QAZ2wsx"
}
```
Проверяем сообщения с юзером 1 (1005261)
```/Dialogs/{user_id}/list```

Меняем настройку ```DialogDbSettings__UseTarantoolDb``` на ```True```
проверям тоже самое только для тарантула


docker exec -it socialnetwork-api-1 apt-get update
docker exec -it socialnetwork-api-1 apt-get install -y telnet
ocker exec -it socialnetwork-api-1 telnet tarantool-db 3301 

docker network ls
docker network inspect otushighloadarchitect_sn-network