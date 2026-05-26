using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int Stock { get; set; }

        [Url(ErrorMessage = "Некорректный URL изображения")]
        public string? ImageUrl { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
