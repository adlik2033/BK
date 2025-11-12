using AutoMapper;
using BK.Models;
using BK.Models.DTO;

namespace BK.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Маппинг для Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());
            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.Ignore());

            // Маппинг для Coupon
            CreateMap<Coupon, CouponDTO>()
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items));
            CreateMap<CreateCouponDTO, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items));
            CreateMap<UpdateCouponDTO, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UsageLimit, opt => opt.Ignore())
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items));
            CreateMap<CouponDTO, Coupon>()
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items));

            // Маппинг для Item
            CreateMap<Item, ItemDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)) // Маппинг опечатки
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            CreateMap<CreateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Игнорируем, т.к. будет загружена по CategoryId

            CreateMap<UpdateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Игнорируем, т.к. будет загружена по CategoryId
            CreateMap<UpdateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    dest.CategoryId = src.CategoryId;
                });

            // Order -> OrderDTO
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Сoupons, opt => opt.MapFrom(src => src.Сoupons)) 
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // CreateOrderDTO -> Order
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id генерируется БД
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Сoupons, opt => opt.MapFrom(src => src.Сoupons)) 
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // UpdateOrderDTO -> Order
            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore()) // OrderNumber обычно не обновляется
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) // TotalAmount обычно пересчитывается
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Сoupons, opt => opt.MapFrom(src => src.Сoupons))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // OrderDTO -> Order
            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Сoupons, opt => opt.MapFrom(src => src.Сoupons))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}