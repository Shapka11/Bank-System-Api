# Лабораторная 5

## Отрабатываемый материал

Реактивное межсервисное взаимодействие, Kafka

## Задача

Интегрировать сервис из Лабораторных 2-4 с сервисом согласования инвойсов.

В сервисе: 
- добавить разделение на обычные и корпоративные счета
- расширить статусную модель инвойсов (добавятся статусы `согласован` и `отклонён`)
- начать писать в `топик счетов`
- начать писать в `топик инвойсов`
- начать читать `топик результатов согласования инвойсов`
- добавить эндпоинт получения пользователей в сервис

В гейтвее:
- добавить эндпоинты сервиса согласования инвойсов

Бухгалтер - пользователь, который назначается на инвойс, только он может согласовать/отклонить этот инвойс. Не является отдельной ролью.

## Функциональные требования

- Ваш сервис должен начать писать в топики `invoice_created` и `account_created` созданные счета и инвойсы
- Ваш сервис должен начать читать топик `approval_result` и изменять статус инвойса в зависимости от ивента
  - После получения события о согласовании инвойса, его можно будет оплатить
  - После получения события о отклонении инвойса, его нельзя будет оплатить или отозвать
- Нельзя отклонять инвойсы на корпоративные счета до тех пор, пока их не согласуют


### Сценарий тестирования

- создание корпоративного счёта (ваш сервис)
- создание инвойса на корпоративный счёт (ваш сервис)
- назначение бухгалтера на инвойс (сервис согласования инвойсов)
- согласование инвойса (сервис согласования инвойса)
- оплата инвойса (ваш сервис)

## Нефункциональные требования

- Для реализации асинхронного взаимодействия использовать библиотеку [Itmo.Dev.Platform.Kafka](https://github.com/itmo-is-dev/platform/tree/master/src/Itmo.Dev.Platform.Kafka) ([документация](https://github.com/itmo-is-dev/platform/blob/feat/kafka-docs/docs/kafka.md))
- Эндпоинты HTTP гейтвея, как новые, так и существующие, должны быть реализованы согласно конвенциям REST

Для развертывания сервиса используйте файл [docker-compose.yaml](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/docker-compose.yaml), он запускает контейнер сервиса, его базу, kafka, а также kafka-ui, который вы можете использовать для просмотра сообщений в топиках и отладки.

Для подключения к kafka вы должны использовать адрес `localhost:9092` если запускаете сервис не в docker контейнере.
Если вы запускаете свой сервис в docker контейнере, то вы должны добавить к нему нетворк `lab5-tools-network` и использовать адрес `kafka:9094`.

Обратите внимание, что в данном файле для конфигурации proto контрактов в kafka-ui используется относительный путь репозитории самого сервиса. Вам будет необходимо изменить этот путь на корректный для вашего репозитория.

```yaml
kafka-ui:
  # ...
  volumes:
    - ./src/Presentation/Lab5.Tools.Presentation.Kafka/protos:/schemas # change path to your local proto directory, ex: `src/lab-5/Kafka/protos:/schemas`
  # ...
```

Данная лабораторная должна быть реализована в папках `service` и  `gateway` соответственно, все необходимые модификации должны быть выполнены в существующем коде.

### Контракты

Схема топика
`созданные счета` – [account_created.proto](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/src/Presentation/Lab5.Tools.Presentation.Kafka/protos/account_created.proto)

Схема топика
`созданные инвойсы` – [invoice_created.proto](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/src/Presentation/Lab5.Tools.Presentation.Kafka/protos/invoice_created.proto)

Схема топика
`результаты согласования инвойсов` – [approval_result.proto](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/src/Infrastructure/Lab5.Tools.Infrastructure.Kafka/protos/approval_result.proto)

Контракты сервиса – [invoice_service.proto](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/src/Presentation/Lab5.Tools.Presentation.Grpc/protos/invoice_service.proto)

Контракт ручки UserService – [user_service.proto](https://github.com/ait-csbe-y28/lab-5-tools/blob/master/src/Infrastructure/Lab5.Tools.Infrastructure.Integrations/protos/user_service.proto)
