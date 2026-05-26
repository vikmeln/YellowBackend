using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Минимум 1 товар")]
        public int Quantity { get; set; }
    }
}
