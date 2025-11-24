namespace BK.Models.DTO
{
    public class OrderUserDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ItemUserDTO> Items { get; set; } = new List<ItemUserDTO>();
        public List<CouponUserDTO> Coupons { get; set; } = new List<CouponUserDTO>();
    }

    public class OrderManagerDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserResponseDTO? User { get; set; }
        public List<ItemUserDTO> Items { get; set; } = new List<ItemUserDTO>();
        public List<CouponUserDTO> Coupons { get; set; } = new List<CouponUserDTO>();
    }

    public class OrderAdminDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserResponseDTO? User { get; set; }
        public List<ItemUserDTO> Items { get; set; } = new List<ItemUserDTO>();
        public List<CouponUserDTO> Coupons { get; set; } = new List<CouponUserDTO>();
    }

    public class CreateOrderDTO
    {
        public List<int> ItemIds { get; set; } = new List<int>();
        public List<int> CouponIds { get; set; } = new List<int>();
    }

    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; } = string.Empty;
    }
}