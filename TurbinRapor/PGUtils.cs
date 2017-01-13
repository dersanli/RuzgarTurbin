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
                    Random random = new Random();
                    for (int i = 0; i < 5; i++)
                    {
                        cmd.CommandText = "INSERT INTO tabel (\"TVALUE\") VALUES ('Hello " + random.Next(0,100) + "')";
                        cmd.ExecuteNonQuery();
                    }
                    random = null;

                    // Retrieve all rows
                    cmd.CommandText = "SELECT \"TVALUE\" FROM tabel";
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