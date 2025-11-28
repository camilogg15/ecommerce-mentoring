namespace CartService.Application.Contracts.Messaging
{
    public interface IMessageHandler
    {
        string EventType { get; }
    }

    public interface IMessageHandler<T> : IMessageHandler
    {
        Task HandleAsync(T message);
    }
}
