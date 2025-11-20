namespace CartService.Application.Contracts.Messaging
{
    public interface IMessageListener
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
