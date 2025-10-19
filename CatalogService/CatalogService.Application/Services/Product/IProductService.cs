using CatalogService.Application.Dtos;

namespace CatalogService.Application.Services.Product
{
    public interface IProductService
    {
        Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> ListAsync(CancellationToken ct = default);
        Task<ProductDto> CreateAsync(string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default);
        Task UpdateAsync(Guid id, string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> ListByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    }
}
