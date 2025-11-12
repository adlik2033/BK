using BK.Models.DTO;

public class ItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } // Исправлена опечатка
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public CategorySimpleDTO Category { get; set; } = null!; // Используем упрощенный DTO для категории
}