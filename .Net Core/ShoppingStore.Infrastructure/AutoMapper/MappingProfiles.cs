using AutoMapper;
using ShoppingStore.Domain.Entities;
using ShoppingStore.Domain.Entities.Identity;
using ShoppingStore.Application.ViewModels.Cart;
using ShoppingStore.Application.ViewModels.Order;
using ShoppingStore.Application.ViewModels.Manage;
using ShoppingStore.Application.ViewModels.Seller;
using ShoppingStore.Application.ViewModels.Slider;
using ShoppingStore.Application.ViewModels.Product;
using ShoppingStore.Application.ViewModels.Category;
using ShoppingStore.Application.ViewModels.Comments;
using ShoppingStore.Application.ViewModels.Newsletter;
using ShoppingStore.Application.ViewModels.CreditCart;
using ShoppingStore.Application.ViewModels.RoleManager;
using ShoppingStore.Application.ViewModels.UserManager;
using ShoppingStore.Application.ViewModels.MessageUser;

namespace ShoppingStore.Infrastructure.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<Role, RolesViewModel>().ReverseMap()
                .ForMember(m => m.UserRoles, opt => opt.Ignore())
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<Product, ProductViewModel>().ReverseMap()
                .ForMember(m => m.SellerId, opt => opt.Ignore());

            CreateMap<Category, CategoryViewModel>().ReverseMap()
                .ForMember(m => m.Parent, opt => opt.Ignore())
                .ForMember(m => m.SubCategories, opt => opt.Ignore())
                .ForMember(m => m.ProductCategories, opt => opt.Ignore());

            CreateMap<Seller, SellerViewModel>().ReverseMap()
                .ForMember(m => m.Products, opt => opt.Ignore());

            CreateMap<Comment, CommentViewModel>().ReverseMap()
                .ForMember(m => m.Product, opt => opt.Ignore());

            CreateMap<Cart, CartViewModel>().ReverseMap()
                .ForMember(m => m.IsComplete, opt => opt.Ignore())
                .ForMember(m => m.BrowserId, opt => opt.Ignore());

            CreateMap<User, EmailViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<User, ProfileViewModel>().ReverseMap()
                .ForMember(m => m.Claims, opt => opt.Ignore());

            CreateMap<MessageUser, MessageUsersViewModel>().ReverseMap();

            CreateMap<CreditCart, CreditCartViewModel>().ReverseMap();

            CreateMap<Newsletter, NewsletterViewModel>().ReverseMap();

            CreateMap<Discount, DiscountViewModel>().ReverseMap();

            CreateMap<Order, OrderViewModel>().ReverseMap();

            CreateMap<Slider, SliderViewModel>().ReverseMap();
        }
    }
}
