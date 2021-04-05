using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;


namespace AdonetExercise
{


    class Program
    {

        const string SqlconnectionString =
                "Server=.;Database=MinionsDB;Integrated Security=true";

        static void Main(string[] args)
        {
            using var connection = new SqlConnection(SqlconnectionString);
            connection.Open();

            int id = int.Parse(Console.ReadLine());

            string query = "EXEC usp_GetOlder @Id";

            using var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("@Id", id);
            sqlCommand.ExecuteNonQuery();

            string selectQuery = "SELECT Name, Age FROM Minions WHERE Id = @Id";
            using var selectCommand = new SqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@Id", id);
            using var reader = selectCommand.ExecuteReader();
            while(reader.Read())
            {
                Console.WriteLine($"{reader[0]} - {reader[1]} years old");
            }

        }

        private static void Problem08(SqlConnection connection, SqlCommand selectCommand, SqlDataReader reader)
        {
            int[] minionsId = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToArray();

            string updateMinionsQuery = @"UPDATE Minions
                                        SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                        WHERE Id = @Id";

            foreach (var id in minionsId)
            {
                using var sqlCommand = new SqlCommand(updateMinionsQuery, connection);
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.ExecuteNonQuery();
            }

            var selectMinions = "SELECT Name, Age FROM Minions";
            selectCommand = new SqlCommand(selectMinions, connection);
            reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader[0]} {reader[1]}");
            }

        }

        private static void Problem07(SqlConnection connection, SqlCommand selectCommand, SqlDataReader reader)
        {
            var minionsQuery = "SELECT Name FROM Minions";
            selectCommand = new SqlCommand(minionsQuery, connection);
            reader = selectCommand.ExecuteReader();
            var minions = new List<String>();

            while (reader.Read())
            {
                minions.Add((string)reader[0]);
            }

            minions.Add("odd");

            int counter = 0;

            for (int i = 0; i < minions.Count / 2; i++)
            {
                Console.WriteLine(minions[0 + counter]);
                Console.WriteLine(minions[minions.Count - 1 - counter]);
                counter++;
            }

            if (minions.Count % 2 != 0)
            {
                Console.WriteLine(minions[minions.Count / 2]);
            }
        }

        private static void Problem06(SqlConnection connection, SqlCommand sqlCommand, SqlCommand sqlDeleteMVCommand, SqlCommand sqlDeleteVCommand)
        {
            int value = int.Parse(Console.ReadLine());

            string evilNameQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
            sqlCommand = new SqlCommand(evilNameQuery, connection);
            sqlCommand.Parameters.AddWithValue("@villainId", value);
            var name = sqlCommand.ExecuteScalar();

            if (name == null)
            {
                Console.WriteLine("No such villian");
            }

            var deleteMinionVillainQuery = @"DELETE FROM MinionsVillains 
                                            WHERE VillainId = @villainId";
            sqlDeleteMVCommand = new SqlCommand(deleteMinionVillainQuery, connection);
            sqlDeleteMVCommand.Parameters.AddWithValue("@vilainId", value);
            var affectedRows = sqlDeleteMVCommand.ExecuteNonQuery();

            var deleteVilliantQuery = @"DELETE FROM Villains
                                        WHERE Id = @villainId";
            sqlDeleteVCommand = new SqlCommand(deleteVilliantQuery, connection);
            sqlDeleteVCommand.Parameters.AddWithValue("@villainId", value);
            var affectedRows1 = sqlDeleteVCommand.ExecuteNonQuery();

            Console.WriteLine($" {name} was deleted.");
            Console.WriteLine($"6 minions were released");
        }

        private static SqlCommand Problem05(SqlConnection connection)
        {
            string countryName = Console.ReadLine();

            string updateTownsQuery = @"UPDATE Towns
                                      SET Name = UPPER(Name)
                                      WHERE CountryCode = (SELECT c.Id FROM Countries AS c 
                                      WHERE c.Name = @countryName)";

            string selectTownsNameQuery = @" SELECT t.Name 
                                             FROM Towns as t
                                             JOIN Countries AS c ON c.Id = t.CountryCode
                                             WHERE c.Name = @countryName";

            var updateCommand = new SqlCommand(updateTownsQuery, connection);
            updateCommand.Parameters.AddWithValue("@countryName", countryName);
            var affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows == 0)
            {
                Console.WriteLine("No town names were affected");
            }
            else
            {
                Console.WriteLine($"{affectedRows} towns names were affected");

                using var selectCommand = new SqlCommand(selectTownsNameQuery, connection);
                selectCommand.Parameters.AddWithValue("@countryName", countryName);

                using (var reader = selectCommand.ExecuteReader())
                {
                    var towns = new List<string>();

                    while (reader.Read())
                    {
                        towns.Add((string)reader[0]);
                    }
                    Console.WriteLine($"[{string.Join(", ", towns)}]");
                }
            }
            return updateCommand;
        }

        private static void proble04(SqlConnection connection)
        {
            string[] input = Console.ReadLine().Split(' ');

            string[] villainInfo = Console.ReadLine().Split(' ');
            string minionName = input[1];
            int age = int.Parse(input[2]);
            string town = input[3];

            int? townId = GetTownId(connection, town);

            if (townId == null)
            {
                string createTownQuery = "INSERT INTO Towns (Name) VALUES (10, @name)";
                using var sqlCommand = new SqlCommand(createTownQuery, connection);
                sqlCommand.Parameters.AddWithValue("@name", town);
                sqlCommand.ExecuteNonQuery();
                townId = GetTownId(connection, town);
                Console.WriteLine($"Town {town} was added to the database");
            }

            string villainName = villainInfo[1];

            int? villainId = GetVillainId(connection, villainName);

            if (villainId == null)
            {
                string createVillain = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                using var sqlCommand = new SqlCommand(createVillain, connection);
                sqlCommand.Parameters.AddWithValue("@villainName", villainName);
                sqlCommand.ExecuteNonQuery();
                GetVillainId(connection, villainName);
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            CreateMinion(connection, minionName, age, townId);
            var minionId = GetMinionId(connection, minionName);

            InsertMinionVillian(connection, villainId, minionId);

            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

        }

        private static void InsertMinionVillian(SqlConnection connection, int? villainId, int? minionId)
        {
            string miniomVillainQuery = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
            using var sqlCommand = new SqlCommand(miniomVillainQuery, connection);
            sqlCommand.Parameters.AddWithValue("@villainId", villainId);
            sqlCommand.Parameters.AddWithValue("@minionId", minionId);
            sqlCommand.ExecuteNonQuery();
            Console.WriteLine($"Successfully added {villainId} to be minion of <VillainName>");
        }

        private static int? GetMinionId(SqlConnection connection, string minionName)
        {
            string minionQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            using var sqlCommand = new SqlCommand(minionQuery, connection);
            sqlCommand.Parameters.AddWithValue("@Name", minionName);
            var minionId = sqlCommand.ExecuteScalar();
            return (int?)minionId;
        }

        private static void CreateMinion(SqlConnection connection, object minionName, int age, int? townId)
        {

            string createMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            using var sqlCommand = new SqlCommand(createMinion, connection);
            sqlCommand.Parameters.AddWithValue("@name", minionName);
            sqlCommand.Parameters.AddWithValue("@age", age);
            sqlCommand.Parameters.AddWithValue("@townId", townId);
            sqlCommand.ExecuteNonQuery();
        }

        private static int? GetVillainId(SqlConnection connection, string villainName)
        {
            string query = "SELECT Id FROM Villains WHERE Name = @Name";
            using var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("@Name", villainName);
            var villainId = sqlCommand.ExecuteScalar();
            return (int?)villainId;
        }

        private static int? GetTownId(SqlConnection connection, string town)
        {
            string townIdQuery = "SELECT Id FROM Towns WHERE Name = @townName";
            using var sqlCommand = new SqlCommand(townIdQuery, connection);
            sqlCommand.Parameters.AddWithValue("@townName", town);
            var townId = sqlCommand.ExecuteScalar();
            return (int?)townId;
        }

        private static void ExtractNameAndCounts(SqlConnection connection)
        {
            string query = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                               FROM Villains AS v 
                               JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                           GROUP BY v.Id, v.Name 
                             HAVING COUNT(mv.VillainId) > 2 
                           ORDER BY COUNT(mv.VillainId)";
            var command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader[0];
                var count = reader[1];
                Console.WriteLine($"{name} {count}");
            }
        }

        private static void ExecuteNonQuery(SqlConnection connection, string query)
        {
            var command = new SqlCommand(query, connection);
            var result = command.ExecuteNonQuery();
        }

        private static void InitialSetup(SqlConnection connection)
        {
            var createTablesStatement = CreateTables();

            foreach (var query in createTablesStatement)
            {
                ExecuteNonQuery(connection, query);
            }

            var insertStatement = InsertIntoTables();

            foreach (var query in insertStatement)
            {
                ExecuteNonQuery(connection, query);
            }
        }

        private static string[] InsertIntoTables()
        {
            var result = new string[]
            {
                "INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')",
                "INSERT INTO Towns ([Name], CountryCode) VALUES ('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)",
                "INSERT INTO Minions (Name,Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)",
                "INSERT INTO EvilnessFactors (Name) VALUES ('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')",
                "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Gru',2),('Victor',1),('Jilly',3),('Miro',4),('Rosen',5),('Dimityr',1),('Dobromir',2)",
                "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (4,2),(1,1),(5,7),(3,5),(2,6),(11,5),(8,4),(9,7),(7,1),(1,3),(7,3),(5,3),(4,3),(1,2),(2,1),(2,7)"
            };

            return result;
        }

        private static string[] CreateTables()
        {
            var result = new string[]
            {
                "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))",
                "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))"
            };
            return result;
        }
    }
}


//string query = "CREATE DATABASE MinionsDB";
//using var command = new SqlCommand(query, connection);
//command.ExecuteNonQuery();

//var command = connection.CreateCommand();
//command.CommandText = "CREATE DATABASE Test123"; ==> create database
//command.ExecuteNonQuery();

//string query = "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))";
//using var command = new SqlCommand(query, connection);
//command.ExecuteNonQuery();

//string query = "INSERT INTO Countries ([Name]) VALUES ('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')";
//using var command = new SqlCommand(query, connection);
//command.ExecuteNonQuery();