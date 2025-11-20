
using BK.Models;

namespace BK
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } 
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
