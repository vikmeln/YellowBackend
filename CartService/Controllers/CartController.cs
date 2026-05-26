using CartService.DTOs;
using CartService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            await _service.AddAsync(GetUserId(), dto);
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            await _service.RemoveAsync(GetUserId(), productId);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            await _service.ClearAsync(GetUserId());
            return NoContent();
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, [FromQuery] int quantity)
        {
            await _service.UpdateQuantityAsync(GetUserId(), productId, quantity);
            return NoContent();
        }
    }
}
