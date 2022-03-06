using FootballBeting.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace FootballBeting
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using var db = new FootballBettingContext();

            db.Database.Migrate();

           // db.Database.EnsureDeleted();

            //db.Database.EnsureCreated();
        }
    }
}
