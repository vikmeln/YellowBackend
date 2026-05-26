using System.ComponentModel.DataAnnotations;

namespace CategoryService.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
