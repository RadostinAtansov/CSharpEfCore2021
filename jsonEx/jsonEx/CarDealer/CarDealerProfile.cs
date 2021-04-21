using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {

            this.CreateMap<ImportModelSuppliers, Supplier>();
            this.CreateMap<ImportModelParts, Part>();

            CreateMap<Customer, ExportCustomers>()
                .ForMember(x => x.BirthDate, y => y.MapFrom(c => c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));

            this.CreateMap<Car, ExportCarsModelInput>();

            this.CreateMap<Supplier, ExportAllSuppliersModel>();
        }
    }
}
