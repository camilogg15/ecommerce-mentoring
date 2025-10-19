using CartService.Domain.Models;
using CartService.Infrastructure.LiteDb;
using Microsoft.Extensions.Options;

namespace CartService.Tests.Integration.Repositories
{
    public class CartRepositoryTests : IDisposable
    {
        private readonly string _testDbPath = "TestCart.db";
        private readonly CartRepository _repository;

        public CartRepositoryTests()
        {
            var options = Options.Create(new LiteDbSettings { DatabasePath = _testDbPath });
            _repository = new CartRepository(options);
        }

        [Fact]
        public void AddItem_And_GetItems_ShouldWorkCorrectly()
        {
            // Arrange
            var cartId = "test_cart";
            var item = new CartItem
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.99m,
                Quantity = 2
            };

            // Act
            _repository.AddItem(cartId, item);
            var result = _repository.GetItems(cartId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Product", result.First().Name);
        }

        [Fact]
        public void RemoveItem_ShouldDeleteItem()
        {
            // Arrange
            var cartId = "test_cart";
            var item = new CartItem { Id = 2, Name = "ToDelete", Price = 5, Quantity = 1 };
            _repository.AddItem(cartId, item);

            // Act
            _repository.RemoveItem(cartId, item.Id);
            var result = _repository.GetItems(cartId);

            // Assert
            Assert.DoesNotContain(result, i => i.Id == item.Id);
        }

        public void Dispose()
        {
            // Cleanup test DB
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }
    }
}
