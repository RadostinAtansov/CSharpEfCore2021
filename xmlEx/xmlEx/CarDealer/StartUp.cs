using CarDealer.Data;
using CarDealer.DataTransfertObject.Input;
using CarDealer.Models;

using System.Linq;

using System;
using CarDealer.XmlHelper;
using System.Collections.Generic;
using AutoMapper;
using CarDealer.DataTransfertObject.Output;
using static CarDealer.DataTransfertObject.Output.SaleOutputModel;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var supplierXml = File.ReadAllText("./Datasets/suppliers.xml");
            //var partsXml = File.ReadAllText("./Datasets/parts.xml");
            //var carsXml = File.ReadAllText("./Datasets/cars.xml");
            //var customerXml = File.ReadAllText("./Datasets/customers.xml");
            //var salesXml = File.ReadAllText("./Datasets/sales.xml");

            ImportSuppliers(context, supplierXml);
            //ImportParts(context, partsXml);
            //ImportCars(context, carsXml);
            //ImportCustomers(context, customerXml);
            //ImportSales(context, salesXml);

            Console.WriteLine(ImportSuppliers(context, supplierXml));

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new SaleOutputModel
                {
                    Car = new CarSaleOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(x => x.Part.Price) -
                    x.Car.PartCars.Sum(x => x.Part.Price) * x.Discount / 100m
                })
                .ToList();

            var result = XmlConverter.Serialize(sales, "sales");

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new CustomerOutputModel
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.Select(x => x.Car).SelectMany(x => x.PartCars).Sum(x => x.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToList();
            var result = XmlConverter.Serialize(customers, "customers");

            return result;

        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            var cars = context.Cars
                .Select(x => new CarPartOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravekkedDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(p => new CarPartInfoOutputModel
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()

                })
                .OrderByDescending(x => x.TravekkedDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new SupplierOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartCount = x.Parts.Count
                })
                .ToList();

            var result = XmlConverter.Serialize(suppliers, "suppliers");

            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new BmwOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenBy(x => x.TravelledDistance)
                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {

            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .Select(c => new CarOutputModel
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();


            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute("cars"));

            //var textWriter = new StringWriter();

            //var ns = new XmlSerializerNamespaces();
            //ns.Add("", "");

            //xmlSerializer.Serialize(textWriter, cars, ns);

            //var result = textWriter.ToString();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";
            var salesDto = XmlConverter.Deserializer<SaleInputModel>(inputXml, root);

            var carsId = context.Cars.Select(x => x.Id).ToList();

            var sales = salesDto
                .Where(x => carsId.Contains(x.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {

            const string root = "Customers";
            InitiliaizerMapper();

            var customersDto = XmlConverter.Deserializer<CustomerInputModel>(inputXml, root);
            var customers = mapper.Map<Customer[]>(customersDto);
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {

            const string root = "Cars";

            //var cars = new List<Car>();
            var carsDto = XmlConverter.Deserializer<CarInputModel>(inputXml, root);
            var allParts = context.Parts.Select(x => x.Id).ToList();


            var cars = carsDto
                .Select(x => new Car
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TraveledDistance,
                    PartCars = x.CarPartsInputModel.Select(x => x.Id)
                    .Distinct()
                    .Intersect(allParts)
                    .Select(pc => new PartCar
                    {
                        PartId = pc
                    })
                    .ToList()

                });

            //var allParts = context.Parts.Select(x => x.Id).ToList();

            //foreach (var currCar in carsDto)
            //{
            //    var distinctedparts = currCar.CarPartsInputModel.Select(x => x.Id).Distinct();
            //    var parts = distinctedparts.Intersect(allParts);

            //    var car = new Car
            //    {
            //        Make = currCar.Make,
            //        Model = currCar.Model,
            //        TravelledDistance = currCar.TraveledDistance,
            //    };

            //    foreach (var part in parts)
            //    {
            //        var partCar = new PartCar
            //        {
            //            PartId = part
            //        };

            //        car.PartCars.Add(partCar);
            //    }

            //    cars.Add(car);
            //}
            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string root = "Parts";

            var partsDto = XmlConverter.Deserializer<PartInputModel>(inputXml, root);

            var supplierIds = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            var parts = partsDto
                .Where(s => supplierIds.Contains(s.SupplierId))
                .Select(x => new Part
                {
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                SupplierId = x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {

            const string root = "Suppliers";

            var supplierDto = XmlConverter.Deserializer<SupplierInputModel>(inputXml, root);

            var suppliers = supplierDto.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.isImporter,
            });

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}"; ;
        }

        public static void InitiliaizerMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}