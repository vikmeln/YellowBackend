using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal? Price { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int? Stock { get; set; }

        public bool? IsActive { get; set; }

        [Url(ErrorMessage = "Некорректный URL изображения")]
        public string? ImageUrl { get; set; }
        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }
    }
}
