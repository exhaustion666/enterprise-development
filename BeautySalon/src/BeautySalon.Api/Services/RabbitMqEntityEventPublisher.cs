using BeautySalon.Contracts.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;

namespace BeautySalon.Api.Services;

/// <summary>
/// Публикует события CRUD-операций в RabbitMQ
/// </summary>
public sealed class RabbitMqEntityEventPublisher(
    IConnection connection,
    ILogger<RabbitMqEntityEventPublisher> logger) : IEntityEventPublisher
{
    private const string ExchangeName = "beautysalon.events";
    private const string QueueName = "beautysalon.events";

    /// <summary>
    /// Публикует событие изменения сущности
    /// </summary>
    public async Task PublishAsync(
        string entityName,
        int entityId,
        string operation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var channel = await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: "#",
                cancellationToken: cancellationToken);

            var message = new
            {
                Entity = entityName,
                Id = entityId,
                Operation = operation,
                OccurredAt = DateTimeOffset.UtcNow
            };

            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: $"{entityName.ToLowerInvariant()}.{operation}",
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Не удалось опубликовать событие {Entity}.{Operation} для ID {EntityId} в RabbitMQ",
                entityName, operation, entityId);
        }
    }
}