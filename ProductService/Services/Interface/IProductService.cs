using ProductService.DTOs;

namespace ProductService.Services.Interface
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<List<ProductResponseDto>> GetActiveAsync();
        Task<List<AdminProductResponseDto>> GetAllAsync();
        Task<AdminProductResponseDto> GetByIdForAdminAsync(int id);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task DecreaseStockAsync(int productId, int quantity);
        Task RestoreStockAsync(int productId, int quantity);
    }
}
