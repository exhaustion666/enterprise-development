using BeautySalon.Api.Middleware;
using BeautySalon.Application.DependencyInjection;
using BeautySalon.Infrastructure.InMemory.DependencyInjection;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddBeautySalonInMemoryInfrastructure();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BeautySalon API v1"));
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();