namespace BK.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public List<Coupon> Coupons { get; set; } = new();
        public List<Item> Items { get; set; } = new();
    }
}