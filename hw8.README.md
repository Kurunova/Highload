Запускаем контейнеры 

```shell
docker-compose -f hw8.docker-compose.yml up -d --build --force-recreate
```

добавляем подключение в Postman с использованием server reflection (локально - grpc://localhost:5062/ или докер контейнер - grpc://localhost:6011/)
message
```
{
    "userId1": "1005261",
    "userId2": "1005259"
}
```
```
{
    "recipientId": "1005259",
    "senderId": "1005261",
    "text": "Hello from GRPC!"
}
```

Заходим на апи http://localhost:5001/swagger/index.html

Логинимся под юзером 1 (1005261) ```/Users/login```
```json
{
    "login": "elenaf1",
        "password": "!QAZ2wsx"
}
```

Авторизуемся с Bearer <token>

Проверяем сообщения с юзером 2 (1005259)
```/Dialogs/{user_id}/list```

Проверяем отправку сообщения юзеру 2 (1005259) ```/Dialogs/{user_id}/send```
```json
{
  "text": "Test me with Grpc!"
}
```
