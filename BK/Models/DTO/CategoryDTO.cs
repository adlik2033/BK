namespace BK.Models.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // не может быть нуллейбл
        public string? Description { get; set; } // может быть нуллейбл
        // коллекционное навигационное свойство
        public List<Item> Items { get; set; } = new();
    }
}
