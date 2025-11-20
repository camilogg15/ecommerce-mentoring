using CartService.Application.Contracts.Messaging;
using CartService.Application.Services.Dispatcher;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartService.Infrastructure.Messaging
{
    public class RabbitConsumer : AsyncDefaultBasicConsumer
    {
        private readonly MessageDispatcher _dispatcher;
        private readonly ILogger _logger;

        public RabbitConsumer(IChannel channel, MessageDispatcher dispatcher, ILoggerFactory loggerFactory)
            : base(channel)
        {
            _dispatcher = dispatcher;
            _logger = loggerFactory.CreateLogger<RabbitConsumer>();
        }

        public override async Task HandleBasicDeliverAsync(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IReadOnlyBasicProperties properties,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var json = Encoding.UTF8.GetString(body.Span);
                _logger.LogDebug("Message received: {Json}", json);

                var envelope = JsonSerializer.Deserialize<EventEnvelope>(json);

                if (envelope == null)
                {
                    _logger.LogWarning("Invalid event envelope.");
                    await Channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken);
                    return;
                }

                await _dispatcher.DispatchAsync(envelope.Type, envelope.Data);
                await Channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing RabbitMQ message");
                await Channel.BasicNackAsync(deliveryTag, multiple: false, requeue: true, cancellationToken);
            }
        }
    }
}
