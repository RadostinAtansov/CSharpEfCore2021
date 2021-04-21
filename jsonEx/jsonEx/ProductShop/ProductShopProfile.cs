using AutoMapper;
using ProductShop.DataTransferObject;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductsInputModel, Product>();
            this.CreateMap<CategoruesInputModels, Category>();
            this.CreateMap<CategoriesProductsInputModel, CategoryProduct>();
        }
    }
}
