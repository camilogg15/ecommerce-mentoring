using CartService.Domain.Models;

namespace CartService.Domain.Interfaces
{
    public interface ICartRepository
    {
        IEnumerable<CartItem> GetItems(string cartId);
        void AddItem(string cartId, CartItem item);
        void RemoveItem(string cartId, int itemId);
    }
}
