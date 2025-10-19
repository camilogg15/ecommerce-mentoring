using CatalogService.Application.Dtos;

namespace CatalogService.Application.Services.Category
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<CategoryDto>> ListAsync(CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(string name, string? imageUrl = null, Guid? parentCategoryId = null, CancellationToken ct = default);
        Task UpdateAsync(Guid id, string name, string? imageUrl = null, Guid? parentCategoryId = null, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
