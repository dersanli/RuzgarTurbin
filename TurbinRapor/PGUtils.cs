using System;
using Npgsql;

namespace TurbinRapor
{
    class PGUtils
    {

        public void TestPGSql()
        {
            using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=!Soyut8745;Database=swtest"))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    // Insert some data
                    cmd.CommandText = "INSERT INTO values (id, textvalue) VALUES (3, 'Hello world')";
                    cmd.ExecuteNonQuery();

                    // Retrieve all rows
                    cmd.CommandText = "SELECT textvalue FROM values";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }
                    }
                }
            }
        }
    }
}