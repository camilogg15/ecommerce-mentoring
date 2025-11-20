using Asp.Versioning;
using CatalogService.Application.Dtos;
using CatalogService.Application.Services.Product;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
                return NotFound();

            // 👇 Add HATEOAS links dynamically
            product.Links = new Dictionary<string, string>
        {
            { "self", Url.Action(nameof(GetById), new { id, version = "1.0" })! },
            { "update", Url.Action(nameof(Update), new { id, version = "1.0" })! },
            { "delete", Url.Action(nameof(Delete), new { id, version = "1.0" })! },
            { "category", Url.Action("GetById", "Categories", new { id = product.CategoryId, version = "1.0" })! }
        };

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] Guid categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.ListByCategoryAsync(categoryId, page, pageSize);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductDto dto)
        {
            var created = await _productService.CreateAsync(dto.Name, dto.CategoryId, dto.Price, dto.Amount, dto.Description, dto.ImageUrl);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            await _productService.UpdateAsync(id, dto.Name, dto.CategoryId, dto.Price, dto.Amount, dto.Description, dto.ImageUrl);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
