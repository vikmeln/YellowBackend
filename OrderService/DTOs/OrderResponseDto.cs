namespace OrderService.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
