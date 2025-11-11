namespace BK.Models.DTO
{
    public class UpdateCategotyDTO
    {
        public string Name { get; set; } = null!; // не может быть нуллейбл
        public string? Description { get; set; } // может быть нуллейбл
    }
}
