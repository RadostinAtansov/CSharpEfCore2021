using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.Data
{
    public class FastFoodYourDbContextDesignTimeFactory : IDesignTimeDbContextFactory<FastFoodContext>
    {

        public FastFoodContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FastFoodContext>();

            //Configuration.GetDirect --> drug nachi za vzemane na connection string
            //TODO: Configuration.GetConnectionString("DefaultConnection")

            builder.UseSqlServer
                ("Server=.;Database=FastFood;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new FastFoodContext(builder.Options);
        }

    }
}
