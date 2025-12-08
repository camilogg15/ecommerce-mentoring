namespace CartService.Domain.Models
{
    public class Cart
    {
        public string Key { get; set; } = string.Empty;
        public IEnumerable<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
