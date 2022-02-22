using Microsoft.Data.SqlClient;
using System;

namespace SixExercise
{
    class Program
    {
        private static string connectionString = "Server=.;" +
            "Database=MinionsDB;" +
            "Integrated Security=true;" +
            "TrustServerCertificate=True;";

        private static SqlConnection connection = new SqlConnection(connectionString);

        private static SqlTransaction transaction;

        static void Main(string[] args)
        {
            connection.Open();
            string id = Console.ReadLine();

            using (connection)
            {
                transaction = connection.BeginTransaction();

                try
                {



                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "SELECT Name FROM Villains WHERE Id = @villainId";
                    command.Parameters.AddWithValue("villainId", id);

                    object value = command.ExecuteScalar();

                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(id), "No such villain was found.");
                    }

                    string villainName = (string)value;
                    command.CommandText = @"DELETE FROM MinionsVillains 
                                             WHERE VillainId = @villainId";

                    int minionsDeleted = command.ExecuteNonQuery();

                    command.CommandText = @"DELETE FROM Villains
                                             WHERE Id = @villainId";

                    command.ExecuteNonQuery();
                    transaction.Commit();

                    Console.WriteLine($"{villainName} was deleted");
                    Console.WriteLine($"{minionsDeleted} was deleted");
                }
                catch(ArgumentNullException ane)
                {
                    try
                    {
                        Console.WriteLine(ane.Message);
                        transaction.Rollback();
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine(er.Message);
                    }
                }
            }
        }
    }
}
