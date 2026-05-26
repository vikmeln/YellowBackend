using OrderService.DTOs;

namespace OrderService.Services.Interface
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateAsync(string userId);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<List<OrderResponseDto>> GetAllAsync();
        Task ChangeStatusAsync(int orderId, OrderStatus status);
        Task CancelAsync(int orderId, string userId);
    }
}
