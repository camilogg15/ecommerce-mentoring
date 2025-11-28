using Asp.Versioning;
using CartService.Application.Services.Cart;
using CartService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace CartService.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{cartId}")]
        [Authorize(Policy = "CustomerOrManager")]
        public IActionResult GetCartItems(string cartId)
        {
            var items = _cartService.GetItems(cartId);
            return Ok(items);
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

        [HttpPost("debug-rabbit")]
        public async Task<IActionResult> DebugRabbit()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var conn = await factory.CreateConnectionAsync();
            using var channel = await conn.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes("""
            { 
              "Type": "ProductUpdated",
              "Data": { "ProductId": 2, "Name": "Moto", "Price": 1200 }
            }
            """);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "catalog.events",
                mandatory: false,
                body: body
            );

            return Ok("Message posted");
        }
    }
}
