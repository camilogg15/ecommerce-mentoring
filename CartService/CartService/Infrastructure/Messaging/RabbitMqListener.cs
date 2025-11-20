using CartService.Application.Contracts.Messaging;
using CartService.Application.Services.Dispatcher;
using RabbitMQ.Client;

namespace CartService.Infrastructure.Messaging
{
    public class RabbitMqListener : IMessageListener, IAsyncDisposable
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly MessageDispatcher _dispatcher;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string QueueName = "catalog.events";

        public RabbitMqListener(ILogger<RabbitMqListener> logger, MessageDispatcher dispatcher, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Queue declared: {Queue}", QueueName);

            var consumer = new RabbitConsumer(_channel, _dispatcher, _loggerFactory);

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("RabbitMQ listener started and consuming queue {Queue}", QueueName);
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
