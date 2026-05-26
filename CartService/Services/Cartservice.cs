using CartService.DTOs;
using CartService.Exceptions;
using CartService.Models;
using CartService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace CartService.Services
{
    public class Cartservice : ICartService
    {
        private readonly CartDbContext _context;
        private readonly HttpClient _httpClient;

        public Cartservice(CartDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task AddAsync(string userId, AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ConflictException("Количество должно быть больше 0");

            var product = await _httpClient.GetFromJsonAsync<ProductDto>(
                $"http://product-service:8080/api/products/{dto.ProductId}");

            if (product == null)
                throw new NotFoundException("Товар не найден");

            var item = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == dto.ProductId);

            if (item != null)
            {
                item.Quantity += dto.Quantity;
            }
            else
            {
                item = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userId, int productId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (item == null)
                throw new NotFoundException("Товар не найден в корзине");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(string userId)
        {
            var items = _context.CartItems.Where(x => x.UserId == userId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItemResponseDto>> GetAsync(string userId)
        {
            var items = await _context.CartItems.Where(x => x.UserId == userId).ToListAsync();

            var result = new List<CartItemResponseDto>();

            foreach (var item in items)
            {
                var product = await _httpClient.GetFromJsonAsync<ProductDto>(
                    $"http://product-service:8080/api/products/{item.ProductId}");

                if (product == null)
                    continue;

                result.Add(new CartItemResponseDto
                {
                    ProductId = item.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Total = product.Price * item.Quantity
                });
            }
            return result;
        }

        public async Task UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (item == null)
                throw new NotFoundException("Товар не найден в корзине");

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            await _context.SaveChangesAsync();
        }
    }
}
