namespace BK.Models
{
    /// <summary>
    /// Заказы
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!; // оплачено, не оплачено и тд
// Покупатель может взять несколько купонов и товаров
        public List<Coupon> Сoupons { get; set; } = new();
        public List<Item> Items = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
