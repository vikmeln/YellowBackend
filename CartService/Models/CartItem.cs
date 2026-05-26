using System.ComponentModel.DataAnnotations;

namespace CartService.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
