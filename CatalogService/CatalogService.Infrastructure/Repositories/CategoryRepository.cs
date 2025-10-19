using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogDbContext _ctx;
        public CategoryRepository(CatalogDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Category entity, CancellationToken ct = default)
        {
            _ctx.Categories.Add(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Category entity, CancellationToken ct = default)
        {
            _ctx.Categories.Remove(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.Categories.AsNoTracking().AnyAsync(c => c.Id == id, ct);
        }

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _ctx.Categories.Include(c => c.Children)
                                        .Include(c => c.Products)
                                        .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<IEnumerable<Category>> ListAsync(CancellationToken ct = default)
        {
            return await _ctx.Categories
                             .Include(c => c.Children)
                             .AsNoTracking()
                             .ToListAsync(ct);
        }

        public async Task UpdateAsync(Category entity, CancellationToken ct = default)
        {
            _ctx.Categories.Update(entity);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
