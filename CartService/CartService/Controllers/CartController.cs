using CartService.Application.Services.Cart;
using CartService.Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{cartId}")]
        public IActionResult GetItems(string cartId)
        {
            var items = _cartService.GetItems(cartId);
            return Ok(items);
        }

        [HttpPost("{cartId}")]
        public IActionResult AddItem(string cartId, [FromBody] CartItem item, [FromServices] IValidator<CartItem> validator)
        {
            var result = validator.Validate(item);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            _cartService.AddItem(cartId, item);
            return Ok();
        }

        [HttpDelete("{cartId}/{itemId}")]
        public IActionResult RemoveItem(string cartId, int itemId)
        {
            _cartService.RemoveItem(cartId, itemId);
            return NoContent();
        }
    }
}
