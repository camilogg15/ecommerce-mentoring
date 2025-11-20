using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Product>> ListAsync(CancellationToken ct = default);
        Task AddAsync(Product entity, CancellationToken ct = default);
        Task UpdateAsync(Product entity, CancellationToken ct = default);
        Task DeleteAsync(Product entity, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Product>> ListByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    }
}
