namespace BK.Models.DTO
{
    public class UpdateCouponDTO
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; } // может быть null
        public string DiscountType { get; set; } = null!; // тип скидки
        public decimal DiscountValue { get; set; } // колличество скидки
        public DateTime ValidFrom { get; set; } // время начала действия
        public DateTime ValidUntil { get; set; } // конец действия
        public int UsageCount { get; set; } // сколько использовано
        public List<Item> items = new(); // коллекционное навигационное свойство для items внутри
    }
}
