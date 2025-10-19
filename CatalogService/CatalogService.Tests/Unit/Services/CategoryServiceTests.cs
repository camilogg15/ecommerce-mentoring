using CatalogService.Application.Services.Category;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using Moq;

namespace CatalogService.Tests.Unit.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _repoMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_AddsCategory_ReturnsDto()
        {
            // arrange
            Category? captured = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                     .Returns<Category, CancellationToken>((c, ct) => { captured = c; return Task.CompletedTask; });

            // act
            var dto = await _service.CreateAsync("Electronics", "http://img", null);

            // assert
            Assert.NotNull(dto);
            Assert.Equal("Electronics", dto.Name);
            Assert.Equal("http://img", dto.ImageUrl);
            Assert.Equal(captured!.Id, dto.Id);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NonExisting_Throws()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(Guid.NewGuid(), "name"));
        }

        [Fact]
        public async Task DeleteAsync_Existing_Deletes()
        {
            var cat = new Category("T", null, null);
            _repoMock.Setup(r => r.GetByIdAsync(cat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(cat);
            _repoMock.Setup(r => r.DeleteAsync(cat, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            await _service.DeleteAsync(cat.Id);

            _repoMock.Verify(r => r.DeleteAsync(cat, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
