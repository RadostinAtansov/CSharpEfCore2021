using Microsoft.EntityFrameworkCore;
using System;

namespace CodeFirstExercise
{
    class StartUp
    {
        static void Main()
        {
            using var db = new StudentSytemContext();

            //db.Database.EnsureCreated();

            db.Database.EnsureDeleted();
            db.Database.Migrate();
        }
    }
}
