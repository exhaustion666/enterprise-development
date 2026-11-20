# BeautySalon

REST API салона красоты на **.NET 10 / ASP.NET Core** с PostgreSQL, RabbitMQ и оркестрацией через .NET Aspire.

## Архитектура

```text
BeautySalon.slnx
├── src/
│   ├── BeautySalon.Domain/                    # Сущности и интерфейсы репозиториев
│   ├── BeautySalon.Contracts/                 # DTO и контракты сервисов
│   ├── BeautySalon.Application/               # CRUD-сервисы и аналитика
│   ├── BeautySalon.Infrastructure.PostgreSql/ # EF Core, PostgreSQL, репозитории, миграции и начальные данные
│   ├── BeautySalon.Infrastructure.InMemory/   # InMemory-репозитории и начальные данные
│   ├── BeautySalon.Api/                       # REST-контроллеры, Swagger, обработка ошибок и RabbitMQ
│   ├── BeautySalon.AppHost/                   # Оркестрация PostgreSQL, RabbitMQ и API через Aspire
│   └── BeautySalon.ServiceDefaults/           # Общие настройки сервисов и health checks
└── tests/
    └── BeautySalon.Tests/                     # Unit- и интеграционные тесты
```

`Domain` содержит сущности `Customer`, `Specialist`, `BeautyService`, `Booking` и интерфейсы репозиториев. `Application` реализует прикладную логику, `Infrastructure.PostgreSql` — хранение данных через Entity Framework Core, `Infrastructure.InMemory` предоставляет дополнительную реализацию CRUD и аналитических репозиториев с хранением данных в памяти, а `Api` предоставляет HTTP-интерфейс.

Строка подключения к PostgreSQL и параметры RabbitMQ передаются в API из Aspire. Миграции и начальные данные применяются при запуске приложения.

## REST API

Базовый префикс маршрутов — `/api`. Идентификаторы создаются базой данных.

|Ресурс|Маршрут|Операции|
|-|-|-|
|Клиенты|`/api/customers`|GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}`|
|Специалисты|`/api/specialists`|GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}`|
|Услуги|`/api/beauty-services`|GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}`|
|Записи|`/api/bookings`|GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}`|
|Аналитика|`/api/analytics/...`|Аналитические запросы|

Создание возвращает `201 Created`, изменение и успешное удаление — `204 No Content`. Для отсутствующей сущности возвращается `404 Not Found`, для конфликтов уникальности и ограничений связей — `409 Conflict`, для некорректных входных данных — `400 Bad Request`.


## PostgreSQL и RabbitMQ

Entity Framework Core работает с PostgreSQL через `AddNpgsqlDbContext` и конфигурацию, переданную Aspire. Миграции применяются автоматически при запуске API; начальные данные задаются через `HasData`.

При изменении сущностей приложение публикует события в RabbitMQ. Параметры подключения к брокеру также передаются через Aspire.

## Обработка ошибок и логирование

`ApiExceptionHandler` преобразует ошибки приложения и PostgreSQL в ответы `ProblemDetails`.

`RequestLoggingMiddleware` записывает HTTP-метод, путь, код ответа и время обработки запроса.