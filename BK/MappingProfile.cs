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
            CreateMap<CreateCategotyDTO, Category>()
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

            CreateMap<Item, ItemDTO>()
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descriprion)) // если исправили опечатку, то убрать эту строку и использовать правильное название
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<CreateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Descriprion, opt => opt.MapFrom(src => src.Description)) // если в модели опечатка, то здесь тоже нужно использовать опечатку, но лучше исправить в модели
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // мы не маппим Category из CreateItemDTO, т.к. там только CategoryId
                .AfterMap((src, dest) => {
                    // Устанавливаем CategoryId, но Category будет загружено при сохранении или через репозиторий
                    dest.CategoryId = src.CategoryId;
                });

            CreateMap<UpdateItemDTO, Item>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Descriprion, opt => opt.MapFrom(src => src.Description))
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