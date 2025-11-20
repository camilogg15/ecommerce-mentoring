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

        public void UpdateProductInCart(string cartId, int productId, string name, decimal price)
        {
            using var db = new LiteDatabase(_databasePath);
            var col = db.GetCollection<CartItem>(SanitizeCartId(cartId));

            var item = col.FindById(productId);
            if (item == null)
                return;

            item.Name = name;
            item.Price = price;

            col.Update(item);
        }

        public IEnumerable<string> GetAllCartIds()
        {
            using var db = new LiteDatabase(_databasePath);
            return db.GetCollectionNames()
                .Where(c => c.StartsWith("cart_"))
                .Select(c => c.Replace("cart_", "").Replace("_", "-"));
        }

        private string SanitizeCartId(string cartId)
        {
            return $"cart_{cartId.Replace("-", "_")}";
        }
    }
}
