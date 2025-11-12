namespace BK.Models.DTO
{
    public class ItemSimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } 
        public decimal Price { get; set; }
    }
}
