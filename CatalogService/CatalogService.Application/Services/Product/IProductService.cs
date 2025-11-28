using CatalogService.Application.Dtos;

namespace CatalogService.Application.Services.Product
{
    public interface IProductService
    {
        Task<ProductDto?> GetAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> ListAsync(CancellationToken ct = default);
        Task<ProductDto> CreateAsync(string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default);
        Task UpdateAsync(int id, string name, Guid categoryId, decimal price, int amount, string? description = null, string? imageUrl = null, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> ListByCategoryAsync(Guid categoryId, int page, int pageSize, CancellationToken ct = default);
        Task DeleteByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    }
}
