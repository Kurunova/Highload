1. Ручное тестирование с помощью браузерной консоли
Браузеры, такие как Chrome или Firefox, имеют встроенную поддержку WebSocket, которую вы можете протестировать из консоли разработчика:

Откройте консоль разработчика (F12) в браузере.
Перейдите на вкладку Console.
Вставьте следующий код:

```javascript
const socket = new WebSocket('ws://localhost:7015/post/feed/posted?access_token=your_jwt_token');

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