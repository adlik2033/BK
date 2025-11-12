namespace BK.Models.DTO
{
    public class CategorySimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}