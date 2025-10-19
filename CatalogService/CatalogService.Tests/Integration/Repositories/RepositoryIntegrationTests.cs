using CatalogService.Domain.Entities;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Tests.Integration.Repositories
{
    public class RepositoryIntegrationTests
    {
        private readonly CatalogDbContext _context;

        public RepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new CatalogDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CategoryRepository_AddAndGet_Works()
        {
            var repo = new CategoryRepository(_context);
            var cat = new Category("Books", null, null);
            await repo.AddAsync(cat);

            var fetched = await repo.GetByIdAsync(cat.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Books", fetched!.Name);
        }

        [Fact]
        public async Task ProductRepository_AddListByCategory_Works()
        {
            var catRepo = new CategoryRepository(_context);
            var prodRepo = new ProductRepository(_context);

            var cat = new Category("Gadgets", null, null);
            await catRepo.AddAsync(cat);

            var p1 = new Product("Phone", cat.Id, 299.99m, 5);
            var p2 = new Product("Watch", cat.Id, 99.99m, 10);

            await prodRepo.AddAsync(p1);
            await prodRepo.AddAsync(p2);

            var byCat = (await prodRepo.ListByCategoryAsync(cat.Id)).ToList();
            Assert.Equal(2, byCat.Count);
            Assert.Contains(byCat, p => p.Name == "Phone");
        }
    }
}
