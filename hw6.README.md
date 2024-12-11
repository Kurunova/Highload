1. Ручное тестирование с помощью браузерной консоли
Браузеры, такие как Chrome или Firefox, имеют встроенную поддержку WebSocket, которую вы можете протестировать из консоли разработчика:

Откройте консоль разработчика (F12) в браузере.
Перейдите на вкладку Console.
Вставьте следующий код:

```javascript
const socket = new WebSocket("ws://localhost:7015/post/feed/posted?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxMDA1MjU3IiwiZ2l2ZW5fbmFtZSI6IkVsZW5hIiwiZmFtaWx5X25hbWUiOiJGIiwiYmlydGhkYXRlIjoiMTk5MC0xMi0wOCIsImNpdHkiOiJCa2siLCJob2JiaWVzIjoibm8iLCJleHAiOjE3MzM5MjI4MzgsImlzcyI6InNvY2lhbG5ldHdvcmsuY29tIiwiYXVkIjoic29jaWFsbmV0d29yay5jb20ifQ.kx7SaTrKUN1zmak64mAq-Wv2dXzAKvp4uhN2KVRc2hg");

socket.onopen = function(event) {
    console.log("WebSocket connection opened:", event);
    socket.send(JSON.stringify({ message: "Hello Server!" }));
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

```
const socket = new WebSocket("ws://localhost:7015/post/feed/posted?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxMDA1MjU3IiwiZ2l2ZW5fbmFtZSI6IkVsZW5hIiwiZmFtaWx5X25hbWUiOiJGIiwiYmlydGhkYXRlIjoiMTk5MC0xMi0wOCIsImNpdHkiOiJCa2siLCJob2JiaWVzIjoibm8iLCJleHAiOjE3MzM5MjI4MzgsImlzcyI6InNvY2lhbG5ldHdvcmsuY29tIiwiYXVkIjoic29jaWFsbmV0d29yay5jb20ifQ.kx7SaTrKUN1zmak64mAq-Wv2dXzAKvp4uhN2KVRc2hg");

socket.onopen = () => {
    console.log("Connected");
};

socket.onmessage = (event) => {
    console.log("Message from server:", event.data);
};
```

Вы увидите сообщения, полученные от сервера, в консоли.

2. Использование WebSocket-клиента, например, Postman
   Postman поддерживает WebSocket-тестирование с удобным интерфейсом.

Перейдите в раздел WebSocket.
Введите адрес вашего WebSocket-сервера, например:

```bash
ws://localhost:7015/post/feed/posted?access_token=your_jwt_token
```

Нажмите "Connect".
После подключения:
Вы можете отправлять сообщения, чтобы проверить реакцию сервера.
Наблюдайте за сообщениями, отправленными сервером, в окне сообщений.