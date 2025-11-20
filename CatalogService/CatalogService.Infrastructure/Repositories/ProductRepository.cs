using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _ctx;
        public ProductRepository(CatalogDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Product entity, CancellationToken ct = default)
        {
            _ctx.Products.Add(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Product entity, CancellationToken ct = default)
        {
            _ctx.Products.Remove(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _ctx.Products.AsNoTracking().AnyAsync(p => p.Id == id, ct);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _ctx.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<Product>> ListAsync(CancellationToken ct = default)
        {
            return await _ctx.Products.Include(p => p.Category).AsNoTracking().ToListAsync(ct);
        }

        public async Task<IEnumerable<Product>> ListByCategoryAsync(Guid categoryId, CancellationToken ct = default)
        {
            return await _ctx.Products
                             .Where(p => p.CategoryId == categoryId)
                             .Include(p => p.Category)
                             .AsNoTracking()
                             .ToListAsync(ct);
        }

        public async Task UpdateAsync(Product entity, CancellationToken ct = default)
        {
            _ctx.Products.Update(entity);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
