namespace BK.Models.DTO
{
    public class AddItemsToCouponDTO
    {
        public List<int> ItemIds { get; set; } = new(); // ID товаров для привязки
    }
}
