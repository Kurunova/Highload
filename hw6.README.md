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

