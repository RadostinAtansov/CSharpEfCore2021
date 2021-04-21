using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObject;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
       static IMapper mapper;

        public static void Main(string[] args)
        {
            var productShopContext = new ProductShopContext();
            //productShopContext.Database.EnsureDeleted();
            //productShopContext.Database.EnsureCreated();

            InitializerMapper();

            //string usersJson = File.ReadAllText("Datasets/users.json");
            //string productsJson = File.ReadAllText("Datasets/products.json");
            //string categoriesJson = File.ReadAllText("Datasets/categories.json");
            //string categoriesProductsJson = File.ReadAllText("Datasets/categories-products.json");
            //ImportUsers(productShopContext, usersJson);
            //ImportProducts(productShopContext, productsJson);
            //ImportCategories(productShopContext, categoriesJson);
            //ImportCategoryProducts(productShopContext, categoriesProductsJson);


            //                      Tuka Si Radko =)

            var result = GetUsersWithProducts(productShopContext);
            Console.WriteLine(result);

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var users = context.Users
                .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
                .Include(x => x.ProductsSold)
                .ToList()
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        products = u.ProductsSold.Where(x => x.BuyerId != null).Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                    }

                })
                .OrderByDescending(x => x.soldProducts.products.Count())
                .ToArray();

            var resultObj = new
            {
                usersCount = users.Count(),
                users = users
            };

            var jsonSerializedSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultJson = JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializedSetting);

            return resultJson;

        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var catByProdCount = context.Categories
             .Select(x => new
             {
                 category = x.Name,
                 productsCount = x.CategoryProducts.Count,
                 averagePrice = x.CategoryProducts.Count == 0 ?
                                              0.ToString("F2") :
                                              x.CategoryProducts.Average(a => a.Product.Price).ToString("F2"),
                 totalRevenue = x.CategoryProducts.Sum(s => s.Product.Price).ToString("F2")
             })
             .OrderByDescending(x => x.productsCount)
             .ToList();

            var result = JsonConvert.SerializeObject(catByProdCount, Formatting.Indented);

            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    soldProducts = user.ProductsSold.Where(p => p.BuyerId != null)
                    .Select(product => new
                    {
                        name = product.Name,
                        price = product.Price,
                        buyerFirstName = product.Buyer.FirstName,
                        buyerLastName = product.Buyer.LastName
                    })
                    .ToList()
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToList();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);

            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + ' ' + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToArray();

            var result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }

        static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializerMapper();
            var DtoCategoryProducts = JsonConvert.DeserializeObject<IEnumerable<CategoriesProductsInputModel>>(inputJson);

            var categoryProduct = mapper.Map<IEnumerable<CategoryProduct>>(DtoCategoryProducts);

            context.AddRange(categoryProduct);
            context.SaveChanges();

            return $"Successfully imported {categoryProduct.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializerMapper();

            var DtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoruesInputModels>>(inputJson);

            var categories = mapper.Map<IEnumerable<Category>>(DtoCategories);

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializerMapper();

            var DtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductsInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(DtoProducts);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {

            InitializerMapper();

            var DtoUser = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(DtoUser);

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        private static void InitializerMapper()
        {
            var config = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }
    }
}