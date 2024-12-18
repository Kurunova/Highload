========================================
Тестирование
========================================

http://localhost:5001/swagger/index.html (или для локального запуска http://localhost:7015/swagger/index.html)

Добавляем юзера 1 ```/Users/register```
```json
{
    "login": "elenaf1",
    "password": "!QAZ2wsx",
    "firstName": "Elena",
    "lastName": "F",
    "birthdate": "2000-01-01T00:00:00.000Z",
    "gender": 2,
    "city": "Bkk",
    "hobbies": "No"
}
```
Добавляем юзера 2 ```/Users/register```
```json
{
    "login": "elenaf2",
    "password": "!QAZ2wsx",
    "firstName": "Elena",
    "lastName": "F",
    "birthdate": "2000-01-01T00:00:00.000Z",
    "gender": 2,
    "city": "Bkk",
    "hobbies": "No"
}
```
Логинимся под юзером 2 ```/Users/login```
```json
{
    "login": "elenaf2",
    "password": "!QAZ2wsx"
}
```
Авторизуемся 
Задать Bearer (apiKey) в сваггер
Запоминаем токен (будет нужен для постмана {tokenUser2})

Добавляем подописку юзера 2 на юзера 1 ```/Users/friend/set/{userId}```
```userId = {userId }```

Логинимся под юзером 1 ```/Users/login```
```json
{
    "login": "elenaf2",
    "password": "!QAZ2wsx"
}
```

Открываем постман и создаем вебсокет 
```bash
ws://localhost:5001/post/feed/posted?access_token=your_jwt_token={tokenUser2} (или с портом 7015 если запуск в отладке)
handshake: {"protocol":"json","version":1}
```

Отправляем пост под юзером 2 в сваггер ```/Posts/create```
```json
{
  "text": "Hello!"
}
```

Проверяем что в вебсокет пришло наше сообщение
Проверяем что в очереди есть сообщение http://localhost:15672/ (guest:guest)

========================================
Инструменты для подключения к вебсокету
========================================

1. Ручное тестирование с помощью браузерной консоли
Браузеры, такие как Chrome или Firefox, имеют встроенную поддержку WebSocket, которую вы можете протестировать из консоли разработчика:

Открыть консоль разработчика (F12) в браузере, вкладка Console.
Вставьте следующий код:

```javascript
const socket = new WebSocket("ws://localhost:7015/post/feed/posted?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxMDA1MjU3IiwiZ2l2ZW5fbmFtZSI6IkVsZW5hIiwiZmFtaWx5X25hbWUiOiJGIiwiYmlydGhkYXRlIjoiMTk5MC0xMi0wOCIsImNpdHkiOiJCa2siLCJob2JiaWVzIjoibm8iLCJleHAiOjE3MzQwMDcwMTUsImlzcyI6InNvY2lhbG5ldHdvcmsuY29tIiwiYXVkIjoic29jaWFsbmV0d29yay5jb20ifQ.vVEJ2tcECCgXpVKJuehHF1OQYc7QdfBi3rD0tbOVp4k");

socket.onopen = function(event) {
    console.log("WebSocket connection opened:", event);
   // SignalR handshake (это сообщение обязательно)
   socket.send(JSON.stringify({ protocol: "json", version: 1 }) + ""); // Символ  обязателен для завершения сообщения
   //socket.send(JSON.stringify({ message: "Hello Server!" }));
};

socket.onmessage = function(event) {
    console.log("Message from server:", event.data);
};

socket.onerror = function(error) {
    console.error("WebSocket error:", error);
};

socket.onclose = function(event) {
    console.log("WebSocket connection closed:", event);
};
```


2. Использование Postman WebSocket

https://www.rafaagahbichelab.dev/articles/signalr-dotnet-postman

Добавить WebSocket. Ввести адрес WebSocket-сервера, например:

```bash
ws://localhost:7015/post/feed/posted?access_token=your_jwt_token={token}
```

Нажать "Connect".
После подключения обязательно отправить такое сообщение для handshake:
{"protocol":"json","version":1}
!!! Обратить внимание на символ в конце ASCII code 0X1E. !!! 
После него активируется OnConnectedAsync.

