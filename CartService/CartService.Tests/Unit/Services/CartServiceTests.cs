using CartService.Domain.Interfaces;
using CartService.Domain.Models;
using Moq;

namespace CartService.Tests.Unit.Services
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _repositoryMock;
        private readonly Application.Services.Cart.CartService _cartService;

        public CartServiceTests()
        {
            _repositoryMock = new Mock<ICartRepository>();
            _cartService = new Application.Services.Cart.CartService(_repositoryMock.Object);
        }

        [Fact]
        public void GetItems_ShouldReturnItemsFromRepository()
        {
            // Arrange
            var cartId = "test-cart";
            var items = new List<CartItem>
            {
                new CartItem { Id = 1, Name = "Product A", Price = 100, Quantity = 1 },
                new CartItem { Id = 2, Name = "Product B", Price = 200, Quantity = 2 }
            };

            _repositoryMock.Setup(r => r.GetItems(cartId)).Returns(items);

            // Act
            var result = _cartService.GetItems(cartId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, item => item.Name == "Product A");
            Assert.Contains(result, item => item.Name == "Product B");
        }

        [Fact]
        public void AddItem_ShouldCallRepository_WhenItemIsValid()
        {
            // Arrange
            var cartId = "test-cart";
            var item = new CartItem { Id = 1, Name = "Product X", Price = 50, Quantity = 1 };

            // Act
            _cartService.AddItem(cartId, item);

            // Assert
            _repositoryMock.Verify(r => r.AddItem(cartId, item), Times.Once);
        }

        [Fact]
        public void AddItem_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            // Arrange
            var cartId = "test-cart";
            var invalidItem = new CartItem { Id = 1, Name = "Product X", Price = 50, Quantity = 0 };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cartService.AddItem(cartId, invalidItem));
            Assert.Equal("Quantity must be positive", ex.Message);

            _repositoryMock.Verify(r => r.AddItem(It.IsAny<string>(), It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public void RemoveItem_ShouldCallRepository()
        {
            // Arrange
            var cartId = "test-cart";
            var itemId = 1;

            // Act
            _cartService.RemoveItem(cartId, itemId);

            // Assert
            _repositoryMock.Verify(r => r.RemoveItem(cartId, itemId), Times.Once);
        }
    }
}
