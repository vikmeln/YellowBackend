using CategoryService.DTOs;
using CategoryService.Exceptions;
using CategoryService.Models;
using CategoryService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Services
{
    public class Categoryservice : ICategoryService
    {
        private readonly CategoryDbContext _context;

        public Categoryservice(CategoryDbContext context)
        {
            _context = context;
        }
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Map(category);
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Категория не найдена");

            if (dto.Name != null)
                category.Name = dto.Name;

            await _context.SaveChangesAsync();
            return Map(category);
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            return await _context.Categories
                .OrderByDescending(c => c.Id)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Категория не найдена");

            return Map(category);
        }

        private static CategoryResponseDto Map(Category c)
        {
            return new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            };
        }
    }
}
