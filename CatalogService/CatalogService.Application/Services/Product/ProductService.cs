using CatalogService.Application.Dtos;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductService(IProductRepository repo, ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
        }

        public async Task<ProductDto> CreateAsync(string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default)
        {
            if (!await _categoryRepo.ExistsAsync(categoryId, ct))
                throw new InvalidOperationException("Category does not exist.");

            var product = new Domain.Entities.Product(name, categoryId, price, amount, description, imageUrl);
            await _repo.AddAsync(product, ct);

            return Map(product);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new InvalidOperationException("Product not found.");
            await _repo.DeleteAsync(existing, ct);
        }

        public async Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, ct);
            return p == null ? null : Map(p);
        }

        public async Task<IEnumerable<ProductDto>> ListAsync(CancellationToken ct = default)
        {
            var list = await _repo.ListAsync(ct);
            return list.Select(Map).ToList();
        }

        public async Task UpdateAsync(Guid id, string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new InvalidOperationException("Product not found.");
            if (!await _categoryRepo.ExistsAsync(categoryId, ct))
                throw new InvalidOperationException("Category does not exist.");

            existing.SetName(name);
            existing.SetCategory(categoryId);
            existing.SetPrice(price);
            existing.SetAmount(amount);
            existing.UpdateDescription(description);
            existing.SetImage(imageUrl);

            await _repo.UpdateAsync(existing, ct);
        }

        public async Task<IEnumerable<ProductDto>> ListByCategoryAsync(Guid categoryId, CancellationToken ct = default)
        {
            var list = await _repo.ListByCategoryAsync(categoryId, ct);
            return list.Select(Map).ToList();
        }

        private static ProductDto Map(Domain.Entities.Product p) => new(p.Id, p.Name, p.Description, p.ImageUrl, p.CategoryId, p.Price, p.Amount);
    }
}
