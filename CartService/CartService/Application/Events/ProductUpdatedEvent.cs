namespace CartService.Application.Events
{
    public record ProductUpdatedEvent(int ProductId, string Name, decimal Price);
}
