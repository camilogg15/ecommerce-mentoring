using CartService.Application.Contracts.Messaging;
using CartService.Application.Events;
using CartService.Application.Services.Cart;

namespace CartService.Application.Handlers
{
    public class ProductUpdatedHandler : IMessageHandler<ProductUpdatedEvent>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public string EventType => "ProductUpdated";

        public ProductUpdatedHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task HandleAsync(ProductUpdatedEvent evt)
        {
            using var scope = _scopeFactory.CreateScope();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            return cartService.UpdateProductInAllCartsAsync(
                evt.ProductId,
                evt.Name,
                evt.Price
            );
        }
    }
}
