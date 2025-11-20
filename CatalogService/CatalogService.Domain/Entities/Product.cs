using CatalogService.Domain.Exceptions;

namespace CatalogService.Domain.Entities
{
    public class Product
    {
        private Product() 
        { 
        
        }

        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Amount { get; private set; }

        public Product(string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null)
        {
            SetName(name);
            SetCategory(categoryId);
            SetPrice(price);
            SetAmount(amount);
            Description = description;
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl?.Trim();
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name is required.");
            if (name.Length > 50)
                throw new DomainException("Product name max length is 50.");
            Name = name.Trim();
        }

        public void SetCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty) throw new DomainException("Category is required.");
            CategoryId = categoryId;
        }

        public void SetPrice(decimal price)
        {
            if (price < 0) throw new DomainException("Price must be >= 0.");
            Price = decimal.Round(price, 2);
        }

        public void SetAmount(int amount)
        {
            if (amount < 0) throw new DomainException("Amount must be >= 0.");
            Amount = amount;
        }

        public void UpdateDescription(string? html)
        {
            Description = html;
        }

        public void SetImage(string? imageUrl)
        {
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl?.Trim();
        }
    }
}
