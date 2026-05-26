using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public OrderStatus Status { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

public enum OrderStatus
{
    Created,
    Paid,
    Shipped,
    Completed,
    Cancelled
}