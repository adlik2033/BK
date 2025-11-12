namespace BK.Models.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<ItemSimpleDTO> Items { get; set; } = new(); // Используем упрощенный DTO для товаров
    }

    public class CreateCategoryDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        // Не включаем Items при создании - товары добавляются отдельно
    }

    public class CategorySimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}