using CatalogService.Application.Dtos;
using CatalogService.Domain.Repositories;

namespace CatalogService.Application.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<CategoryDto> CreateAsync(string name, string? imageUrl = null, Guid? parentCategoryId = null, CancellationToken ct = default)
        {
            var category = new Domain.Entities.Category(name, imageUrl, parentCategoryId);
            await _repo.AddAsync(category, ct);
            return Map(category);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new InvalidOperationException("Category not found.");
            await _repo.DeleteAsync(existing, ct);
        }

        public async Task<CategoryDto?> GetAsync(Guid id, CancellationToken ct = default)
        {
            var cat = await _repo.GetByIdAsync(id, ct);
            return cat == null ? null : Map(cat);
        }

        public async Task<IEnumerable<CategoryDto>> ListAsync(CancellationToken ct = default)
        {
            var cats = await _repo.ListAsync(ct);
            return cats.Select(Map).ToList();
        }

        public async Task UpdateAsync(Guid id, string name, string? imageUrl = null, Guid? parentCategoryId = null, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) throw new InvalidOperationException("Category not found.");
            existing.SetName(name);
            existing.SetImage(imageUrl);
            existing.SetParent(parentCategoryId);
            await _repo.UpdateAsync(existing, ct);
        }

        private static CategoryDto Map(Domain.Entities.Category c) => new(c.Id, c.Name, c.ImageUrl, c.ParentCategoryId);
    }
}
