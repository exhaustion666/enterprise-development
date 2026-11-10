# BeautySalon

REST API салона красоты на **.NET 10 / ASP.NET Core**.

## Архитектура

```text
BeautySalon.slnx
├── src/
│   ├── BeautySalon.Domain/                    # Сущности, исключения, интерфейсы репозиториев
│   ├── BeautySalon.Contracts/                 # DTO и контракты сервисов
│   ├── BeautySalon.Application/               # CRUD, маппинг, аналитика
│   ├── BeautySalon.Infrastructure.InMemory/  # InMemory-репозитории и начальные данные
│   └── BeautySalon.Api/                       # Контроллеры, DI, обработка ошибок, Swagger
└── tests/
    └── BeautySalon.Tests/                     # xUnit: аналитические тесты
```

`Domain` содержит сущности `Customer`, `Specialist`, `BeautyService`, `Booking` и контракт `IRepository<TEntity>`. `Infrastructure.InMemory` реализует репозитории на основе `Dictionary<int, TEntity>`. API связывает компоненты через DI.

Swagger UI доступен по адресу `https://localhost:7240/swagger` или `http://localhost:5279/swagger` (порты заданы в `launchSettings.json`).

## REST API

Базовый адрес — `/api`. Все CRUD-операции работают через типизированные DTO. Идентификаторы назначаются репозиторием автоматически.

| Ресурс | Маршрут | Операции |
|---|---|---|
| Клиенты | `/api/customers` | GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}` |
| Специалисты | `/api/specialists` | GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}` |
| Услуги | `/api/beauty-services` | GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}` |
| Записи | `/api/bookings` | GET, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}` |

Успешное создание возвращает `201 Created` и заголовок `Location`. Успешное изменение и удаление возвращают `204 No Content`. Для отсутствующего объекта возвращается `404 Not Found`. Отрицательные ID проверяются атрибутами `[Range]` и автоматически дают `400 Bad Request` благодаря `[ApiController]`.

Входные DTO проверяются через атрибуты `Required`, `Range`, `StringLength`, `Phone` и `EnumDataType`.

## Обработка ошибок и логирование

Ожидаемые ошибки преобразуются в `ProblemDetails`: `400` — некорректный запрос, `404` — сущность не найдена, `409` — конфликт связей. Неожиданные ошибки возвращают `500` без раскрытия внутренних подробностей. `RequestLoggingMiddleware` записывает HTTP-метод, путь, код ответа и время обработки.