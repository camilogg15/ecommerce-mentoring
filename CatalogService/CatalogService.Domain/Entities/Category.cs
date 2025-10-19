using CatalogService.Domain.Exceptions;

namespace CatalogService.Domain.Entities
{
    public class Category
    {
        private Category() 
        { 
        
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = null!;
        public string? ImageUrl { get; private set; }
        public Guid? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }
        public ICollection<Category> Children { get; private set; } = new List<Category>();
        public ICollection<Product> Products { get; private set; } = new List<Product>();
        public Category(string name, string? imageUrl = null, Guid? parentCategoryId = null)
        {
            SetName(name);
            ImageUrl = imageUrl;
            ParentCategoryId = parentCategoryId;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");
            if (name.Length > 50)
                throw new DomainException("Category name max length is 50.");
            Name = name.Trim();
        }

        public void SetImage(string? imageUrl)
        {
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl!.Trim();
        }

        public void SetParent(Guid? parentId)
        {
            ParentCategoryId = parentId;
        }
    }
}
