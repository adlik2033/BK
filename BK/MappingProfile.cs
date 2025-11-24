using AutoMapper;
using BK.Models;
using BK.Models.DTO;

namespace BK.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserResponseDTO>();
            CreateMap<User, UserProfileDTO>();
            CreateMap<User, AdminUserDTO>();

            // Role mappings
            CreateMap<Role, RoleResponseDTO>();
            CreateMap<CreateRoleDTO, Role>();

            // Category mappings
            CreateMap<Category, CategorySimpleDTO>();
            CreateMap<Category, CategoryUserDTO>();
            CreateMap<Category, CategoryManagerDTO>();
            CreateMap<Category, CategoryAdminDTO>();
            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // Item mappings
            CreateMap<Item, ItemSimpleDTO>();
            CreateMap<Item, ItemUserDTO>();
            CreateMap<Item, ItemManagerDTO>();
            CreateMap<Item, ItemAdminDTO>();
            CreateMap<CreateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<UpdateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // Coupon mappings
            CreateMap<Coupon, CouponUserDTO>();
            CreateMap<Coupon, CouponManagerDTO>();
            CreateMap<Coupon, CouponAdminDTO>();
            CreateMap<CreateCouponDTO, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
            CreateMap<UpdateCouponDTO, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsageLimit, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // Order mappings
            CreateMap<Order, OrderUserDTO>();
            CreateMap<Order, OrderManagerDTO>();
            CreateMap<Order, OrderAdminDTO>();
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Создан"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Coupons, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Order mappings
            CreateMap<Order, OrderUserDTO>()
                .ForMember(dest => dest.Coupons, opt => opt.MapFrom(src => src.Coupons))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<Order, OrderManagerDTO>()
                .ForMember(dest => dest.Coupons, opt => opt.MapFrom(src => src.Coupons))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User != null ? new UserResponseDTO
                {
                    Id = src.User.Id,
                    Login = src.User.Login,
                    Email = src.User.Email,
                    Role = src.User.Role.Name,
                    CreatedAt = src.User.CreatedAt
                } : null));

            CreateMap<Order, OrderAdminDTO>()
                .ForMember(dest => dest.Coupons, opt => opt.MapFrom(src => src.Coupons))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User != null ? new UserResponseDTO
                {
                    Id = src.User.Id,
                    Login = src.User.Login,
                    Email = src.User.Email,
                    Role = src.User.Role.Name,
                    CreatedAt = src.User.CreatedAt
                } : null));
        }
    }
}