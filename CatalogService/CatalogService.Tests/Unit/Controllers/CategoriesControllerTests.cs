using CatalogService.API.Controllers;
using CatalogService.Application.Dtos;
using CatalogService.Application.Services.Category;
using CatalogService.Application.Services.Product;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CatalogService.Tests.Unit.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock = new();
        private readonly Mock<IProductService> _productServiceMock = new();
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _controller = new CategoriesController(_categoryServiceMock.Object, _productServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithCategories()
        {
            // Arrange
            var categories = new List<CategoryDto>
            {
                new(Guid.NewGuid(), "Tech", null, null),
                new(Guid.NewGuid(), "Home", null, null)
            };

            _categoryServiceMock
                .Setup(s => s.ListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithCategory()
        {
            // Arrange
            var dto = new CategoryDto(Guid.Empty, "Electronics", "img.jpg", null);
            var created = new CategoryDto(Guid.NewGuid(), dto.Name, dto.ImageUrl, dto.ParentCategoryId);

            _categoryServiceMock
                .Setup(s => s.CreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync((string name, string? imageUrl, Guid? parentId, CancellationToken ct) =>
                    new CategoryDto(Guid.NewGuid(), name, imageUrl, parentId)
                );

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<CategoryDto>(createdResult.Value);
            Assert.Equal("Electronics", value.Name);
        }

        [Fact]
        public async Task Update_CallsService_AndReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CategoryDto(id, "Updated", null, null);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            _categoryServiceMock
                .Setup(s => s.UpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                ))
                .Returns(Task.CompletedTask);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_CallsBothServices_AndReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var result = await _controller.Delete(id);

            // Assert
            _productServiceMock.Verify(s => s.DeleteByCategoryAsync(id, default), Times.Once);
            _categoryServiceMock.Verify(s => s.DeleteAsync(id, default), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
