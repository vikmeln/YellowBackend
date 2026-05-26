using CategoryService.DTOs;

namespace CategoryService.Services.Interface
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(int id);
    }
}
