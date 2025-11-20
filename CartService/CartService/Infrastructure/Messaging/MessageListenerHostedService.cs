using CartService.Application.Contracts.Messaging;

namespace CartService.Infrastructure.Messaging
{
    public class MessageListenerHostedService : IHostedService
    {
        private readonly IMessageListener _listener;

        public MessageListenerHostedService(IMessageListener listener)
        {
            _listener = listener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => _listener.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
