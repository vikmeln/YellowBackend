using CartService.DTOs;

namespace CartService.Services.Interface
{
    public interface ICartService
    {
        Task AddAsync(string userId, AddToCartDto dto);
        Task RemoveAsync(string userId, int productId);
        Task ClearAsync(string userId);
        Task<List<CartItemResponseDto>> GetAsync(string userId);
        Task UpdateQuantityAsync(string userId, int productId, int quantity);
    }
}
