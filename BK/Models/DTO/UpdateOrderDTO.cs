namespace BK.Models.DTO
{
    public class UpdateOrderDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!; // оплачено, не оплачено и тд
                                                    // Покупатель может взять несколько купонов и товаров
        public List<Coupon> Сoupons { get; set; } = new();
        public List<Item> Items = new();
    }
}
