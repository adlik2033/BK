namespace BK.Models.DTO
{
    public class CreateItemDTO
    {
        public string Name { get; set; } = null!; // не может быть нуллейбл
        public string? Descriprion { get; set; } // описание
        public decimal Price { get; set; }
        public int CategoryId { get; set; } // внешний ключ зависимости сущностей
        public Category Category { get; set; } = null!; // навигационное свойство
    }
}
