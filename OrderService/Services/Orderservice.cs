using Microsoft.EntityFrameworkCore;
using OrderService.DTOs;
using OrderService.Exceptions;
using OrderService.Models;
using OrderService.Services.Interface;
using System.Net.Http.Headers;

namespace OrderService.Services
{
    public class Orderservice : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Orderservice(OrderDbContext context, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OrderResponseDto> CreateAsync(string userId)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Нет токена");

            var pureToken = token.Replace("Bearer ", "");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var cartRequest = new HttpRequestMessage(
                HttpMethod.Get,
                "http://cart-service:8080/api/cart");

            cartRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pureToken);

            var cartResponse = await _httpClient.SendAsync(cartRequest);

            if (!cartResponse.IsSuccessStatusCode)
            {
                var error = await cartResponse.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка получения корзины: {error}");
            }

            var cart = await cartResponse.Content.ReadFromJsonAsync<List<CartItemDto>>();

            if (cart == null || !cart.Any())
                throw new ConflictException("Корзина пуста");

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = 0,
                OrderItems = new List<OrderItem>()
            };

            _context.Orders.Add(order);

            foreach (var item in cart)
            {
                var productRequest = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"http://product-service:8080/api/products/{item.ProductId}");

                productRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pureToken);

                var productResponse = await _httpClient.SendAsync(productRequest);

                if (!productResponse.IsSuccessStatusCode)
                {
                    var error = await productResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка получения товара: {error}");
                }

                var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

                if (product == null)
                    throw new NotFoundException("Товар не найден");

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = product.Price
                });

                order.TotalPrice += product.Price * item.Quantity;

                var decreaseRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    "http://product-service:8080/api/products/decrease-stock");

                decreaseRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pureToken);

                decreaseRequest.Content = JsonContent.Create(new
                {
                    productId = item.ProductId,
                    quantity = item.Quantity
                });

                var decreaseResponse = await _httpClient.SendAsync(decreaseRequest);

                if (!decreaseResponse.IsSuccessStatusCode)
                {
                    var error = await decreaseResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка уменьшения склада: {error}");
                }
            }

            await _context.SaveChangesAsync();

            var clearRequest = new HttpRequestMessage(
                HttpMethod.Delete,
                "http://cart-service:8080/api/cart");

            clearRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pureToken);

            var clearResponse = await _httpClient.SendAsync(clearRequest);

            if (!clearResponse.IsSuccessStatusCode)
            {
                var error = await clearResponse.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка очистки корзины: {error}");
            }
            await transaction.CommitAsync();
            return Map(order);
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            return orders.Select(Map).ToList();
        }

        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
            return orders.Select(Map).ToList();
        }

        private bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            return current switch
            {
                OrderStatus.Created => next == OrderStatus.Paid || next == OrderStatus.Cancelled,
                OrderStatus.Paid => next == OrderStatus.Shipped || next == OrderStatus.Cancelled,
                OrderStatus.Shipped => next == OrderStatus.Completed,
                OrderStatus.Completed => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }

        public async Task ChangeStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException("Заказ не найден");

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Нельзя изменить финальный статус");

            if (!IsValidTransition(order.Status, newStatus))
                throw new ConflictException(
                    $"Нельзя изменить статус с {order.Status} на {newStatus}");

            if (newStatus == OrderStatus.Cancelled)
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                var pureToken = token?.Replace("Bearer ", "");

                foreach (var item in order.OrderItems)
                {
                    var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        "http://product-service:8080/api/products/restore-stock");

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", pureToken);

                    request.Content = JsonContent.Create(new
                    {
                        productId = item.ProductId,
                        quantity = item.Quantity
                    });

                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Ошибка возврата товара: {error}");
                    }
                }
            }
            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int orderId, string userId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException("Заказ не найден");

            if (order.UserId != userId)
                throw new ForbiddenException("Нет доступа");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Заказ уже отменён");

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Completed)
                throw new ConflictException("Нельзя отменить");

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var pureToken = token?.Replace("Bearer ", "");

            foreach (var item in order.OrderItems)
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "http://product-service:8080/api/products/restore-stock");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", pureToken);

                request.Content = JsonContent.Create(new
                {
                    productId = item.ProductId,
                    quantity = item.Quantity
                });

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка возврата товара: {error}");
                }
            }
            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
        }

        private static OrderResponseDto Map(Order o)
        {
            return new OrderResponseDto
            {
                Id = o.Id,
                Status = o.Status.ToString(),
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems?.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    PriceAtPurchase = i.PriceAtPurchase
                }).ToList()
            };
        }
    }
}
