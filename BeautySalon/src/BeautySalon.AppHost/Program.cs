using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18.3")
    .WithPassword(builder.AddParameter("postgres-password", secret: true));

var useVolumes = builder.Configuration["UseVolumes"] != "false";
if (useVolumes)
{
    postgres.WithDataVolume("beautysalon-postgres-data");
}

var database = postgres.AddDatabase("beautysalon");

var messagingUser = builder.AddParameter("messaging-user");
var messagingPassword = builder.AddParameter("messaging-password", secret: true);

var messaging = builder.AddRabbitMQ(
        "messaging",
        messagingUser,
        messagingPassword)
    .WithManagementPlugin();

if (useVolumes)
{
    messaging.WithDataVolume("beautysalon-rabbitmq-data");
}

builder.AddProject<Projects.BeautySalon_Api>("api")
    .WithReference(database)
    .WithReference(messaging)
    .WaitFor(database)
    .WaitFor(messaging)
    .WithHttpHealthCheck("/health");

builder.Build().Run();

internal static class AppHostDefaults
{
    public static IDistributedApplicationBuilder AddServiceDefaults(
        this IDistributedApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        return builder;
    }
}