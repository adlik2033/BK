public class CreateItemDTO
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } // Исправлена опечатка
    public decimal Price { get; set; }
    public int CategoryId { get; set; } // Только ID категории
}
