using CartService.Domain.Interfaces;
using CartService.Domain.Models;
using LiteDB;
using Microsoft.Extensions.Options;

namespace CartService.Infrastructure.LiteDb
{
    public class CartRepository : ICartRepository
    {
        private readonly string _databasePath;

        public CartRepository(IOptions<LiteDbSettings> settings)
        {
            _databasePath = settings.Value.DatabasePath;
        }

        public IEnumerable<CartItem> GetItems(string cartId)
        {
            using var db = new LiteDatabase(_databasePath);
            var col = db.GetCollection<CartItem>(SanitizeCartId(cartId));
            return col.FindAll().ToList();
        }

        public void AddItem(string cartId, CartItem item)
        {
            using var db = new LiteDatabase(_databasePath);
            var col = db.GetCollection<CartItem>(SanitizeCartId(cartId));
            col.Upsert(item);
        }

        public void RemoveItem(string cartId, int itemId)
        {
            using var db = new LiteDatabase(_databasePath);
            var col = db.GetCollection<CartItem>(SanitizeCartId(cartId));
            col.Delete(itemId);
        }

        private string SanitizeCartId(string cartId)
        {
            return $"cart_{cartId.Replace("-", "_")}";
        }
    }
}
