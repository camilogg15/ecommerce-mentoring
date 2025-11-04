using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Category>> ListAsync(CancellationToken ct = default);
        Task AddAsync(Category entity, CancellationToken ct = default);
        Task UpdateAsync(Category entity, CancellationToken ct = default);
        Task DeleteAsync(Category entity, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}
