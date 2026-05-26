using System.ComponentModel.DataAnnotations;

namespace CategoryService.DTOs
{
    public class UpdateCategoryDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
    }
}
