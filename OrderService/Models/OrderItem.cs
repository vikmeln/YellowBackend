using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal PriceAtPurchase { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
