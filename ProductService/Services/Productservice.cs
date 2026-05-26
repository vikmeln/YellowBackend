using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.DTOs;
using ProductService.Exceptions;
using ProductService.Models;
using ProductService.Services.Interface;
using System.Text.Json;

namespace ProductService.Services
{
    public class Productservice : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public Productservice(ProductDbContext context, IHttpClientFactory httpClientFactory, IDistributedCache cache)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient("CategoryService");
            _cache = cache;
        }

        private async Task<bool> CategoryExists(int categoryId)
        {
            var response = await _httpClient.GetAsync($"/api/categories/{categoryId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            var exists = await CategoryExists(dto.CategoryId);
            if (!exists)
                throw new NotFoundException("Категория не найдена");
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                IsActive = true
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product:{product.Id}");
            return MapToProductResponse(product);
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Товар не найден");

            if (dto.Name != null)
                product.Name = dto.Name;

            if (dto.Description != null)
                product.Description = dto.Description;

            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (dto.Stock.HasValue)
                product.Stock = dto.Stock.Value;

            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value;

            if (dto.ImageUrl != null)
                product.ImageUrl = dto.ImageUrl;

            if (dto.CategoryId.HasValue)
            {
                var exists = await CategoryExists(dto.CategoryId.Value);
                if (!exists)
                    throw new NotFoundException("Категория не найдена");
                product.CategoryId = dto.CategoryId.Value;
            }
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product:{id}");
            return MapToProductResponse(product);
        }

        public async Task ActivateAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Товар не найден");

            product.IsActive = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new NotFoundException("Товар не найден");

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductResponseDto>> GetActiveAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Id)
                .Select(p => MapToProductResponse(p))
                .ToListAsync();
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var cacheKey = $"product:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached != null)
            {
                return JsonSerializer.Deserialize<ProductResponseDto>(cached)!;
            }

            var product = await _context.Products
                .Where(p => p.IsActive && p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
                throw new NotFoundException("Товар не найден");

            var result = MapToProductResponse(product);

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            return result;
        }

        public async Task<List<AdminProductResponseDto>> GetAllAsync()
        {
            return await _context.Products
                .OrderByDescending(p => p.Id)
                .Select(p => new AdminProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();
        }

        public async Task<AdminProductResponseDto> GetByIdForAdminAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new NotFoundException("Товар не найден");

            return new AdminProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId
            };
        }

        private static ProductResponseDto MapToProductResponse(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }

        public async Task DecreaseStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new NotFoundException("Товар не найден");

            if (product.Stock < quantity)
                throw new ConflictException("Недостаточно товара");

            product.Stock -= quantity;
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product:{productId}");
        }

        public async Task RestoreStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new NotFoundException("Товар не найден");

            product.Stock += quantity;
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"product:{productId}");
        }
    }
}
