using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }

        [Url(ErrorMessage = "Некорректный URL изображения")]
        public string? ImageUrl { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public int CategoryId { get; set; }
    }
}
