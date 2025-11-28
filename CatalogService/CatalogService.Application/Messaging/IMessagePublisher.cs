namespace CatalogService.Application.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string eventType, object payload, CancellationToken cancellationToken);
    }
}
