namespace CatalogService.Application.Dtos
{
    public class ProductDto
    {
        public ProductDto(int id, string name, string? description, string? imageUrl, Guid categoryId, decimal price, int amount)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            CategoryId = categoryId;
            Price = price;
            Amount = amount;               
        }

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public Guid CategoryId { get; set; }

        public Dictionary<string, string>? Links { get; set; }
    }
}
