using CatalogService.Application.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;

namespace CatalogService.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly ILogger<RabbitMqPublisher> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string _exchangeName = "catalog.events.exchange";

        public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;
        }

        private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
        {
            if (_connection != null && _channel != null)
                return;

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                _exchangeName,
                ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );
        }

        public async Task PublishAsync(string eventType, object payload, CancellationToken cancellationToken)
        {
            await EnsureConnectedAsync(cancellationToken);

            var message = new
            {
                Type = eventType,
                Data = payload
            };

            var json = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);

            await _channel!.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: "",
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Published event {EventType}: {Payload}", eventType, json);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
    }
}
