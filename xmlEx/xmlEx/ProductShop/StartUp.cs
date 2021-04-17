using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Export.GetUsersWithProducts;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XmlHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
            //db.Database.EnsureCreated();
            //db.Database.EnsureCreated();

            //var usersXml = File.ReadAllText("Datasets/users.xml");
            //var productsXml = File.ReadAllText("Datasets/products.xml");
            //var categories = File.ReadAllText("Datasets/categories.xml");
            //var categoriesProductsXml = File.ReadAllText("Datasets/categories-products.xml");

            InitializeMapper();

            //  Tuka siiiii ---------------------------------------------- Tuk ------------------------------------------------------

            //ImportUsers(db, usersXml);
            //ImportProducts(db, productsXml);
            //ImportCategories(db, categories);
            //ImportCategoryProducts(db, categoriesProductsXml);

            var result = GetUsersWithProducts(db);
            Console.WriteLine(result);

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = new UsersCount()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any(a => a.BuyerId != null)),
                UserOutput = context.Users
                    //.AsEnumerable() // or else inmemory error from judge
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Take(10)
                    .Select(u => new UserModel()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,

                        SoldProducts = new SoldProducts()
                        {
                            Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                            Products = u.ProductsSold
                                .Where(ps => ps.Buyer != null)
                                .Select(ps => new Products()
                                {
                                    Name = ps.Name,
                                    Price = ps.Price
                                })
                                .OrderByDescending(p => p.Price)
                                .ToList()
                        }
                    })
                    .ToList()
            };

            var result = XmlConverter.Serialize(users, "Users");

            return result;
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {

            var categoriesByCount = context.Categories
                .Select(c => new GetCategoriesByProductCountOutputModel
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(a => a.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(tp => tp.Product.Price)
                })
                .OrderByDescending(p => p.Count)
                .ThenBy(r => r.TotalRevenue)
                .ToList();

            var result = XmlConverter.Serialize(categoriesByCount, "Categories");

            return result;
        }


        public static string GetSoldProducts(ProductShopContext context)
        {

            var usersWithOneSoldItem = context.Users
                .Where(x => x.ProductsSold.Count > 0)
                .Select(x => new GetSoldProductsOutput
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new SoldProductsInfoOutput
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    
                    .ToArray()
                })
                .OrderBy(ln => ln.LastName)
                .ThenBy(fn => fn.FirstName)
                .Take(5)
                .ToList();

            var result = XmlConverter.Serialize(usersWithOneSoldItem, "Users");

            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {

            var products = context.Products
                .Where(pr => pr.Price >= 500 && pr.Price <= 1000)
                .Select(x => new GetProductsInRangeOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            //var root = ;
            var result = XmlConverter.Serialize(products, "Products");

            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {

            string root = "CategoryProducts";

            var categoriesProductsDtos = XmlConverter.Deserializer<InputCategoriesProductsModel>(inputXml, root);

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var categoryProductDto in categoriesProductsDtos)
            {
                if (context.Categories.Any(c => c.Id == categoryProductDto.CategoryId) && // check if both exist
                    context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryProductDto.CategoryId,
                        ProductId = categoryProductDto.ProductId
                    };
                    categoryProducts.Add(categoryProduct);
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }


        public static string ImportCategories(ProductShopContext context, string inputXml)
        {

            string root = "Categories";
            var categoriesDto = XmlConverter.Deserializer<InputCategoriesModel>(inputXml, root);

            var categories = categoriesDto.Select(x => new Category
            {
                Name = x.Name
            })
            .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {

            string root = "Products";

            var productsDto = XmlConverter.Deserializer<InputProductsModel>(inputXml, root);

            var products = productsDto.Select(x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                SellerId = x.SellerId,
                BuyerId = x.BuyerId
            })
            .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            string root = "Users";

            var usersDto = XmlConverter.Deserializer<InputUsersModel>(inputXml, root);

            var users = usersDto.Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age
            })
            .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<ProductShopProfile>(); });
        }
    }
}