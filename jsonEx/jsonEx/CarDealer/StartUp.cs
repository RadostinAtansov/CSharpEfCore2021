using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {

        static IMapper mapper;

        public static void Main(string[] args)
        {
            // Tuka si Radko ::) --------------------------------------------------------------------------//

            var carDealerContext = new CarDealerContext();
            //carDealerContext.Database.EnsureDeleted();
            //carDealerContext.Database.EnsureCreated();

            InitializeMapper();

            //var supplierJson = File.ReadAllText("Datasets/suppliers.json");
            //var partJson = File.ReadAllText("Datasets/parts.json");
            //var carsJson = File.ReadAllText("Datasets/cars.json");
            //var customersJson = File.ReadAllText("Datasets/customers.json");
            //var salesJson = File.ReadAllText("Datasets/sales.json");

            //ImportSuppliers(carDealerContext, supplierJson);
            //ImportParts(carDealerContext, partJson);
            //ImportCars(carDealerContext, carsJson);
            //ImportCustomers(carDealerContext, customersJson);
            //ImportSales(carDealerContext, salesJson);

            var result = GetTotalSalesByCustomer(carDealerContext);
            Console.WriteLine(result);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesDiscount = context
                .Sales
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        x.Car.Make,
                        x.Car.Model,
                        x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = x.Discount.ToString("F2"),
                    price = x.Car.PartCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    priceWithDiscount =
                        (x.Car.PartCars.Sum(pc => pc.Part.Price) -
                         (x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100)).ToString("F2")

                })
                .ToList();

            var result = JsonConvert.SerializeObject(salesDiscount, Formatting.Indented);

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            var totalCars = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            var result = JsonConvert.SerializeObject(totalCars, Formatting.Indented);

            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                 .Select(c => new
                 {
                     car = new
                     {
                         c.Make,
                         c.Model,
                         c.TravelledDistance
                     },
                     parts = c.PartCars.Select(pc => new
                     {
                         pc.Part.Name,
                         Price = pc.Part.Price.ToString("F2")
                     })
                 })
                 .ToList();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .ProjectTo<ExportAllSuppliersModel>()
                .ToList();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {

            var cars = context
                .Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ProjectTo<ExportCarsModelInput>()
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
                
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {

            var customers = context
                .Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .ProjectTo<ExportCustomers>()
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var dtoSales = JsonConvert.DeserializeObject<IEnumerable<Sale>>(inputJson);

            var sales = dtoSales.Select(x => new Sale
            {
                CarId = x.CarId,
                CustomerId = x.CustomerId,
                Discount = x.Discount
            })
                .ToList();

            context.AddRange(sales);
            context.SaveChanges();


            return $"Successfully imported {sales.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var dtoCustomers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);

            var customers = dtoCustomers.Select(x => new Customer
            {
                Name = x.Name,
                BirthDate = x.BirthDate,
                IsYoungDriver = x.IsYoungDriver
            })
             .ToList();

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var dtoCars = JsonConvert.DeserializeObject<IEnumerable<ImportModelCars>>(inputJson);

            var listOfCars = new List<Car>();

            foreach (var car in dtoCars)
            {
                var currCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                foreach (var partId in car?.PartsId.Distinct())
                {
                    currCar.PartCars.Add(new PartCar { PartId = partId });
                }
                listOfCars.Add(currCar);
            }
            context.Cars.AddRange(listOfCars);
            context.SaveChanges();

            return $"Successfully imported {listOfCars.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {

            var supplierIds = context.Suppliers
             .Select(x => x.Id)
             .ToArray();

            var parts = JsonConvert.DeserializeObject<IEnumerable<Part>>(inputJson)
                .Where(s => supplierIds.Contains(s.SupplierId))
                .ToList();                         

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {

            var dtoSuppliers = JsonConvert.DeserializeObject<IEnumerable<Supplier>>(inputJson);

            context.Suppliers.AddRange(dtoSuppliers);
            context.SaveChanges();

            return $"Successfully imported {dtoSuppliers.Count()}.";
        }

        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
        }

        //private static void InitialirerMapper()
        //{
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.AddProfile<CarDealerProfile>();
        //    });
        //    mapper = config.CreateMapper();
        //}
    }
}