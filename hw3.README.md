Запустить hw3.master_setup.sh вручную:

Зайти в контейнер:
docker exec -it socialnetwork-db-master bash

Проверить что скрипт доступен:
ls -l /replication/hw3.master_setup.sh

Права на выполнение (если нужно):
chmod +x /replication/hw3.master_setup.sh

Запустить скрипт:
/replication/hw3.master_setup.sh