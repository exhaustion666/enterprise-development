using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Npgsql;
using Xunit;

namespace BeautySalon.Tests;

/// <summary>
/// Приложение для интеграционных тестов
/// </summary>
public sealed class AspireIntegrationFixture : IAsyncLifetime
{
    /// <summary>
    /// Максимальное время ожидания запуска приложения
    /// </summary>
    private static readonly TimeSpan _startupTimeout = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Тестовое приложение
    /// </summary>
    private IAsyncDisposable? _appHost;

    /// <summary>
    /// Запущенное приложение
    /// </summary>
    private DistributedApplication? _application;

    /// <summary>
    /// HTTP-клиент для обращения к API
    /// </summary>
    private HttpClient? _httpClient;

    /// <summary>
    /// Подключение к тестовой базе данных
    /// </summary>
    private string? _connectionString;

    /// <summary>
    /// Возвращает HTTP-клиент
    /// </summary>
    public HttpClient HttpClient => _httpClient
        ?? throw new InvalidOperationException("HTTP-клиент ещё не инициализирован.");

    /// <summary>
    /// Возвращает строку подключения к базе данных
    /// </summary>
    public string ConnectionString => _connectionString
        ?? throw new InvalidOperationException("Строка подключения PostgreSQL ещё не инициализирована.");

    /// <summary>
    /// Запуск AppHost, PostgreSQL, RabbitMQ и API
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        using var startupTimeout = new CancellationTokenSource(_startupTimeout);
        var cancellationToken = startupTimeout.Token;

        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.BeautySalon_AppHost>(
            [
                "--environment=Development",
                "UseVolumes=false",
                "Parameters:postgres-password=Integration-test-password-123!",
                "Parameters:messaging-user=integration-test",
                "Parameters:messaging-password=Integration-test-rabbitmq-password-123!"
            ], cancellationToken);

        _appHost = appHost;
        _application = await appHost.BuildAsync(cancellationToken);
        await _application.StartAsync(cancellationToken);

        await _application.ResourceNotifications.WaitForResourceHealthyAsync(
            "api", cancellationToken);

        _httpClient = _application.CreateHttpClient("api", "http");

        _connectionString = await _application.GetConnectionStringAsync(
                "beautysalon",
                cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                "Aspire не предоставил строку подключения PostgreSQL.");
    }

    /// <summary>
    /// Остановка приложения и освобождение ресурсов окружения
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();

        if (_application is not null)
        {
            await _application.DisposeAsync();
        }

        if (_appHost is not null)
        {
            await _appHost.DisposeAsync();
        }
    }
}

/// <summary>
/// Сквозные проверки API, PostgreSQL, RabbitMQ и связей между сущностями
/// </summary>
public sealed class AspireIntegrationTests(AspireIntegrationFixture fixture)
    : IClassFixture<AspireIntegrationFixture>
{
    private static readonly DateTimeOffset _firstBookingStart =
        new(2030, 1, 15, 10, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset _updatedBookingStart =
        new(2030, 1, 16, 10, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Проверка запуска приложения, начальных данных PostgreSQL и аналитического запроса
    /// </summary>
    [Fact]
    public async Task AppHostStartsAndPostgreSqlContainsSeedData()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var connection = await OpenDatabaseConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            "SELECT COUNT(*) FROM \"Customers\"", connection);

        var customerCount = (long)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException(
                "Не удалось получить количество клиентов."));

        Assert.True(customerCount >= 10);

        using (var response = await fixture.HttpClient.GetAsync(
            "/api/customers", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var customers = await response.Content.ReadFromJsonAsync<JsonElement>(
                cancellationToken);

            Assert.True(customers.GetArrayLength() >= 10);
        }

        using var analyticsResponse = await fixture.HttpClient.GetAsync(
            "/api/analytics/top-services?count=5", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, analyticsResponse.StatusCode);
    }

    /// <summary>
    /// Проверка создания, получения, изменения и удаления клиента
    /// </summary>
    [Fact]
    public async Task CustomerSupportsCrudOperations()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var customerRequest = new
        {
            lastName = "Волкова",
            firstName = "Екатерина",
            patronymic = "Андреевна",
            dateOfBirth = "1994-06-18",
            gender = "Female",
            phone = "+79001234567"
        };

        using var createResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/customers", customerRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var customerId = createdCustomer.GetProperty("id").GetInt32();
        Assert.True(customerId > 0);

        await using (var connection = await OpenDatabaseConnectionAsync(cancellationToken))
        {
            await using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM \"Customers\" WHERE \"Id\" = @id", connection);
            command.Parameters.AddWithValue("id", customerId);

            var inserted = (long)(await command.ExecuteScalarAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    "Не удалось проверить добавление клиента."));

            Assert.Equal(1L, inserted);
        }

        using (var getResponse = await fixture.HttpClient.GetAsync(
            $"/api/customers/{customerId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        var updatedCustomerRequest = new
        {
            lastName = "Волкова",
            firstName = "Екатерина",
            patronymic = "Андреевна",
            dateOfBirth = "1994-06-18",
            gender = "Female",
            phone = "+79007654321"
        };

        using (var updateResponse = await fixture.HttpClient.PutAsJsonAsync(
            $"/api/customers/{customerId}",
            updatedCustomerRequest,
            cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        using (var deleteResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/customers/{customerId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        await using (var connection = await OpenDatabaseConnectionAsync(cancellationToken))
        {
            await using var command = new NpgsqlCommand(
                "SELECT COUNT(*) FROM \"Customers\" WHERE \"Id\" = @id", connection);
            command.Parameters.AddWithValue("id", customerId);

            var remaining = (long)(await command.ExecuteScalarAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    "Не удалось проверить удаление клиента."));

            Assert.Equal(0L, remaining);
        }
    }

    /// <summary>
    /// Проверка CRUD специалиста и отклонение повторного номера паспорта
    /// </summary>
    [Fact]
    public async Task SpecialistSupportsCrudOperationsAndRejectsDuplicatePassport()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        const string phone = "+79001112233";
        const string passportNumber = "9876543210";

        var specialistRequest = new
        {
            lastName = "Миронов",
            firstName = "Александр",
            patronymic = "Сергеевич",
            dateOfBirth = "1987-09-23",
            gender = "Male",
            phone,
            passportNumber,
            specialization = "Косметолог",
            experienceYears = 8
        };

        using var createResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/specialists", specialistRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdSpecialist = await createResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var specialistId = createdSpecialist.GetProperty("id").GetInt32();
        Assert.True(specialistId > 0);

        using (var duplicateResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/specialists", specialistRequest, cancellationToken))
        {
            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
        }

        var updatedSpecialistRequest = new
        {
            lastName = "Миронов",
            firstName = "Александр",
            patronymic = "Сергеевич",
            dateOfBirth = "1987-09-23",
            gender = "Male",
            phone,
            passportNumber,
            specialization = "Массажист",
            experienceYears = 9
        };

        using (var updateResponse = await fixture.HttpClient.PutAsJsonAsync(
            $"/api/specialists/{specialistId}",
            updatedSpecialistRequest,
            cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        using (var getResponse = await fixture.HttpClient.GetAsync(
            $"/api/specialists/{specialistId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        using var deleteResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/specialists/{specialistId}", cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    /// <summary>
    /// Проверка создания, получения, изменения и удаления услуги
    /// </summary>
    [Fact]
    public async Task BeautyServiceSupportsCrudOperations()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var serviceRequest = new
        {
            name = "Чистка лица",
            category = "Косметология",
            price = 2500m,
            durationMinutes = 60
        };

        using var createResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/beauty-services", serviceRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdService = await createResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var serviceId = createdService.GetProperty("id").GetInt32();
        Assert.True(serviceId > 0);

        using (var getResponse = await fixture.HttpClient.GetAsync(
            $"/api/beauty-services/{serviceId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        var updatedServiceRequest = new
        {
            name = "Массаж лица",
            category = "Уход за лицом",
            price = 1800m,
            durationMinutes = 30
        };

        using (var updateResponse = await fixture.HttpClient.PutAsJsonAsync(
            $"/api/beauty-services/{serviceId}",
            updatedServiceRequest,
            cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        using var deleteResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/beauty-services/{serviceId}", cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    /// <summary>
    /// Проверка CRUD записи и запрета удаления клиента на которого она ссылается
    /// </summary>
    [Fact]
    public async Task BookingSupportsCrudOperationsAndRestrictsCustomerDeletion()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var specialistRequest = new
        {
            lastName = "Миронов",
            firstName = "Александр",
            patronymic = "Сергеевич",
            dateOfBirth = "1987-09-23",
            gender = "Male",
            phone = "+79001112234",
            passportNumber = "9876543211",
            specialization = "Косметолог",
            experienceYears = 8
        };

        using var createSpecialistResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/specialists", specialistRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createSpecialistResponse.StatusCode);

        var createdSpecialist = await createSpecialistResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var specialistId = createdSpecialist.GetProperty("id").GetInt32();

        var serviceRequest = new
        {
            name = "Чистка лица",
            category = "Косметология",
            price = 2500m,
            durationMinutes = 60
        };

        using var createServiceResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/beauty-services", serviceRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createServiceResponse.StatusCode);

        var createdService = await createServiceResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var serviceId = createdService.GetProperty("id").GetInt32();

        var customerRequest = new
        {
            lastName = "Волкова",
            firstName = "Екатерина",
            patronymic = "Андреевна",
            dateOfBirth = "1994-06-18",
            gender = "Female",
            phone = "+79009876543"
        };

        using var createCustomerResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/customers", customerRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createCustomerResponse.StatusCode);

        var createdCustomer = await createCustomerResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var customerId = createdCustomer.GetProperty("id").GetInt32();

        var bookingRequest = new
        {
            startAt = _firstBookingStart,
            specialistId,
            customerId,
            beautyServiceId = serviceId,
            isRegularCustomer = false
        };

        using var createBookingResponse = await fixture.HttpClient.PostAsJsonAsync(
            "/api/bookings", bookingRequest, cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createBookingResponse.StatusCode);

        var createdBooking = await createBookingResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken);
        var bookingId = createdBooking.GetProperty("id").GetInt32();

        await using var connection = await OpenDatabaseConnectionAsync(cancellationToken);

        await using (var command = new NpgsqlCommand(
            """
            SELECT COUNT(*)
            FROM "Bookings"
            WHERE "Id" = @bookingId AND "CustomerId" = @customerId
            """, connection))
        {
            command.Parameters.AddWithValue("bookingId", bookingId);
            command.Parameters.AddWithValue("customerId", customerId);

            var linkedBookingCount = (long)(await command.ExecuteScalarAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    "Не удалось проверить связь записи с клиентом."));

            Assert.Equal(1L, linkedBookingCount);
        }

        using (var getResponse = await fixture.HttpClient.GetAsync(
            $"/api/bookings/{bookingId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        using (var conflictingDeleteResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/customers/{customerId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.Conflict, conflictingDeleteResponse.StatusCode);
        }

        var updatedBookingRequest = new
        {
            startAt = _updatedBookingStart,
            specialistId,
            customerId,
            beautyServiceId = serviceId,
            isRegularCustomer = true
        };

        using (var updateResponse = await fixture.HttpClient.PutAsJsonAsync(
            $"/api/bookings/{bookingId}",
            updatedBookingRequest,
            cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        }

        using (var deleteBookingResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/bookings/{bookingId}", cancellationToken))
        {
            Assert.Equal(HttpStatusCode.NoContent, deleteBookingResponse.StatusCode);
        }

        using var deleteCustomerResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/customers/{customerId}", cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteCustomerResponse.StatusCode);

        using var deleteSpecialistResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/specialists/{specialistId}", cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteSpecialistResponse.StatusCode);

        using var deleteServiceResponse = await fixture.HttpClient.DeleteAsync(
            $"/api/beauty-services/{serviceId}", cancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, deleteServiceResponse.StatusCode);
    }

    /// <summary>
    /// Подключение к тестовой базе данных
    /// </summary>
    private async Task<NpgsqlConnection> OpenDatabaseConnectionAsync(
        CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(fixture.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}