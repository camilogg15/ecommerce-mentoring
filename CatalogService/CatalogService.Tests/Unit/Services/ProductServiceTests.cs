using CatalogService.Application.Messaging;
using CatalogService.Application.Services.Product;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using Moq;

namespace CatalogService.Tests.Unit.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _prodRepo = new();
        private readonly Mock<ICategoryRepository> _catRepo = new();
        private readonly Mock<IMessagePublisher> _publisher = new();
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _service = new ProductService(_prodRepo.Object, _catRepo.Object, _publisher.Object);
        }

        [Fact]
        public async Task CreateAsync_WithMissingCategory_Throws()
        {
            _catRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync("P", Guid.NewGuid(), 10m, 1));
        }

        [Fact]
        public async Task CreateAsync_WithCategory_Creates()
        {
            _catRepo.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(true);

            Product? captured = null;
            _prodRepo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<System.Threading.CancellationToken>()))
                     .Callback<Product, System.Threading.CancellationToken>((p, ct) => captured = p)
                     .Returns(Task.CompletedTask);

            var dto = await _service.CreateAsync("Prod", Guid.NewGuid(), 5.5m, 10);

            Assert.NotNull(captured);
            Assert.Equal(dto.Id, captured.Id);
            Assert.Equal("Prod", captured.Name);
            Assert.Equal(5.5m, captured.Price);
        }
    }
}
