namespace BK.Models
{
    /// <summary>
    /// Сами товары
    /// </summary>
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // не может быть нуллейбл
        public string? Descriprion { get; set; } // описание
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int CategoryId { get; set; } // внешний ключ зависимости сущностей
        public Category Category { get; set; } = null!; // навигационное свойство
    }
}
