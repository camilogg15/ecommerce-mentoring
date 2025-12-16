using CartService.Application.Services.Dispatcher;

using RabbitMQ.Client;

namespace CartService.Infrastructure.Messaging
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly MessageDispatcher _dispatcher;

        private IConnection? _connection;
        private IChannel? _consumerChannel;

        private const string _queueName = "catalog.events";

        public RabbitMqListener(ILogger<RabbitMqListener> logger, MessageDispatcher dispatcher, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            // Retry loop
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Connecting to RabbitMQ...");
                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                    _consumerChannel = await _connection.CreateChannelAsync();

                    await _consumerChannel.QueueDeclareAsync(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null,
                        cancellationToken: stoppingToken);

                    await _consumerChannel.QueueBindAsync(
                        queue: _queueName,
                        exchange: "catalog.events.exchange",
                        routingKey: "",
                        cancellationToken: stoppingToken);

                    await _consumerChannel.BasicQosAsync(
                        prefetchSize: 0,
                        prefetchCount: 1,
                        global: false,
                        cancellationToken: stoppingToken);                    

                    var consumer = new RabbitConsumer(_consumerChannel, _dispatcher, _loggerFactory);

                    await _consumerChannel.BasicConsumeAsync(
                        queue: _queueName,
                        autoAck: false,
                        consumer: consumer,
                        cancellationToken: stoppingToken);                    

                    _logger.LogInformation(
                        "RabbitMQ listener connected and consuming queue {Queue}",
                        _queueName);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Shutdown normal
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "RabbitMQ not ready yet. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ listener...");

            if (_consumerChannel is not null)
            {
                await _consumerChannel.CloseAsync();
                _consumerChannel.Dispose();
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
