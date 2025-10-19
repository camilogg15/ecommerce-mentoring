using CartService.Domain.Interfaces;
using CartService.Domain.Models;

namespace CartService.Application.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<CartItem> GetItems(string cartId)
        {
            return _repository.GetItems(cartId);
        }        

        public void AddItem(string cartId, CartItem item)
        {
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be positive");

            _repository.AddItem(cartId, item);
        }

        public void RemoveItem(string cartId, int itemId)
        {
            _repository.RemoveItem(cartId, itemId);
        }            
    }
}
