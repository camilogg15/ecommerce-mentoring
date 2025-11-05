using CartService.Application.Services.Cart;
using CartService.Controllers.v1;
using CartService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartService.Tests.Unit.Controllers
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _controller = new CartController(_cartServiceMock.Object);
        }

        [Fact]
        public void GetCart_ReturnsCartWithItems()
        {
            // Arrange
            var cartId = "abc123";
            var items = new List<CartItem>
            {
                new CartItem { Id = 1, Name = "Item 1", Price = 10 },
                new CartItem { Id = 2, Name = "Item 2", Price = 20 }
            };
            _cartServiceMock.Setup(s => s.GetItems(cartId)).Returns(items);

            // Act
            var result = _controller.GetCart(cartId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var cart = Assert.IsType<Cart>(okResult.Value);
            Assert.Equal(cartId, cart.Key);
            Assert.Equal(2, cart.Items.Count());
        }

        [Fact]
        public void AddItem_CallsServiceAndReturnsOk()
        {
            // Arrange
            var cartId = "abc123";
            var item = new CartItem { Id = 1, Name = "New Item", Price = 15 };

            // Act
            var result = _controller.AddItem(cartId, item);

            // Assert
            _cartServiceMock.Verify(s => s.AddItem(cartId, item), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteItem_CallsServiceAndReturnsOk()
        {
            // Arrange
            var cartId = "abc123";
            var itemId = 1;

            // Act
            var result = _controller.DeleteItem(cartId, itemId);

            // Assert
            _cartServiceMock.Verify(s => s.RemoveItem(cartId, itemId), Times.Once);
            Assert.IsType<OkResult>(result);
        }
    }
}
