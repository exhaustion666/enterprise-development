using BeautySalon.Api.Middleware;
using BeautySalon.Api.Services;
using BeautySalon.Application.DependencyInjection;
using BeautySalon.Contracts.Interfaces;
using BeautySalon.Infrastructure.PostgreSql;
using BeautySalon.Infrastructure.PostgreSql.DependencyInjection;
using BeautySalon.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<BeautySalonDbContext>("beautysalon");
builder.AddRabbitMQClient("messaging", settings => settings.DisableAutoActivation = false);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BeautySalon",
        Description = "Cалон красоты: клиенты, мастера, услуги, записи и аналитика."
    });

    foreach (var xmlFile in Directory.EnumerateFiles(AppContext.BaseDirectory, "BeautySalon.*.xml"))
    {
        options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddBeautySalonApplication();
builder.Services.AddBeautySalonPostgreSqlInfrastructure();
builder.Services.AddSingleton<IEntityEventPublisher, RabbitMqEntityEventPublisher>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
    await dbContext.Database.MigrateAsync();

    await dbContext.Database.ExecuteSqlRawAsync("""
        SELECT setval(pg_get_serial_sequence('"Customers"', 'Id'),
            COALESCE((SELECT MAX("Id") FROM "Customers"), 0) + 1, false);
        """);
    await dbContext.Database.ExecuteSqlRawAsync("""
        SELECT setval(pg_get_serial_sequence('"Specialists"', 'Id'),
            COALESCE((SELECT MAX("Id") FROM "Specialists"), 0) + 1, false);
        """);
    await dbContext.Database.ExecuteSqlRawAsync("""
        SELECT setval(pg_get_serial_sequence('"BeautyServices"', 'Id'),
            COALESCE((SELECT MAX("Id") FROM "BeautyServices"), 0) + 1, false);
        """);
    await dbContext.Database.ExecuteSqlRawAsync("""
        SELECT setval(pg_get_serial_sequence('"Bookings"', 'Id'),
            COALESCE((SELECT MAX("Id") FROM "Bookings"), 0) + 1, false);
        """);
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BeautySalon API v1"));
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.MapControllers();
app.Run();