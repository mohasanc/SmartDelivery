using AutoMapper;
using SmartDelivery.Application.DTOs.Categories;
using SmartDelivery.Application.DTOs.MenuItems;
using SmartDelivery.Application.DTOs.Orders;
using SmartDelivery.Application.DTOs.Restaurants;
using SmartDelivery.Application.DTOs.Users;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<User, UserSummaryDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<UpdateProfileRequest, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Restaurant Mappings 
            //CreateMap<Restaurant, RestaurantDto>()
            //    .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : string.Empty));

            CreateMap<Restaurant, RestaurantDto>()
                .ForCtorParam(
                    "OwnerName",
                    opt => opt.MapFrom(src =>
                        src.Owner != null
                            ? src.Owner.FullName
                            : string.Empty
                    )
                );

            CreateMap<Restaurant, RestaurantSummaryDto>();

            CreateMap<CreateRestaurantRequest, Restaurant>();
            CreateMap<UpdateRestaurantRequest, Restaurant>();

            // Category Mappings
            CreateMap<Category, CategoryDto>()
                .ForCtorParam(
                    "MenuItemCount",
                    opt => opt.MapFrom(src => src.MenuItems.Count(m => m.IsActive))
                );
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // MenuItem Mappings
            CreateMap<MenuItem, MenuItemDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            CreateMap<CreateMenuItemRequest, MenuItem>();
            CreateMap<UpdateMenuItemRequest, MenuItem>();

            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : string.Empty))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.FullName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<Order, OrderSummaryDto>()
                 .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : string.Empty))
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                 .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems != null ? src.OrderItems.Sum(i => i.Quantity) : 0));

            CreateMap<OrderItem, OrderItemDto>();

        }
    }
}
