using Asp.Versioning;
using CatalogService.Application.Dtos;
using CatalogService.Application.Services.Category;
using CatalogService.Application.Services.Product;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoriesController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.ListAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto)
        {
            var created = await _categoryService.CreateAsync(dto.Name, dto.ImageUrl, dto.ParentCategoryId);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CategoryDto dto)
        {
            await _categoryService.UpdateAsync(id, dto.Name, dto.ImageUrl, dto.ParentCategoryId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteByCategoryAsync(id);
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
