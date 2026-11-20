using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BeautySalon.Infrastructure.PostgreSql.DesignTime;

/// <summary>
/// Создаёт контекст для команд dotnet ef без запуска Aspire
/// </summary>
public sealed class BeautySalonDbContextFactory : IDesignTimeDbContextFactory<BeautySalonDbContext>
{
    /// <summary>
    /// Создаёт контекст по временной строке подключения из переменной окружения
    /// </summary>
    public BeautySalonDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__beautysalon");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Для dotnet ef задайте переменную окружения ConnectionStrings__beautysalon.");
        }

        var options = new DbContextOptionsBuilder<BeautySalonDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new BeautySalonDbContext(options);
    }
}