namespace BK.Models
{
    /// <summary>
    /// Категории товаров
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // не может быть нуллейбл
        public string? Description { get; set; } // может быть нуллейбл
        public bool IsActive { get; set; } = true; // изначально активен
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        // коллекционное навигационное свойство
        public List<Item> Items { get; set; } = new();

    }
}
