using CartService.Domain.Models;

namespace CartService.Application.Services.Cart
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetItems(string cartId);
        void AddItem(string cartId, CartItem item);
        void RemoveItem(string cartId, int itemId);
    }
}
