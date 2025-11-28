using Asp.Versioning;
using CartService.Application.Services.Cart;
using CartService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers.v1
{
    /// <summary>
    /// Version 1: Returns a full cart model.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet()]
        [Authorize(Policy = "CustomerOrManager")]
        public IActionResult GetCartsIds()
        {
            var items = _cartService.GetItems();
            return Ok(items);
        }

        /// <summary>Get cart info by key.</summary>
        [HttpGet("{cartId}")]
        [Authorize(Policy = "CustomerOrManager")]
        public IActionResult GetCart(string cartId)
        {
            var items = _cartService.GetItems(cartId);
            return Ok(new Cart { Key = cartId, Items = items });
        }

        [HttpPost("{cartId}/items")]
        [Authorize(Policy = "CustomerOrManager")]
        public IActionResult AddItem(string cartId, [FromBody] CartItem item)
        {
            _cartService.AddItem(cartId, item);
            return Ok();
        }

        [HttpDelete("{cartId}/items/{itemId}")]
        [Authorize(Policy = "CustomerOrManager")]
        public IActionResult DeleteItem(string cartId, int itemId)
        {
            _cartService.RemoveItem(cartId, itemId);
            return Ok();
        }
    }
}
