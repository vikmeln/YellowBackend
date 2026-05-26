using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services.Interface;
using System.Security.Claims;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            var order = await _service.CreateAsync(userId);
            return Ok(order);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _service.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            await _service.CancelAsync(id, userId);
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] OrderStatus status)
        {
            await _service.ChangeStatusAsync(id, status);
            return NoContent();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? throw new Exception("UserId not found");
        }
    }
}
