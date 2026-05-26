using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class KnigaDB
    {
        public static List<Kniga> GetVseKnigi()
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.AuthorId, b.IsFrozen,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId
                    WHERE b.IsFrozen = 0
                    GROUP BY b.BookId, b.Title, b.Description,
                             b.CoverPath, b.AuthorId, b.IsFrozen, u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(ChitatKnigu(reader));
                }
            }

            return spisok;
        }

        public static List<Kniga> Poisk(string tekst)
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.AuthorId, b.IsFrozen,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId
                    WHERE b.IsFrozen = 0
                      AND (b.Title LIKE @tekst OR u.DisplayName LIKE @tekst)
                    GROUP BY b.BookId, b.Title, b.Description,
                             b.CoverPath, b.AuthorId, b.IsFrozen, u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@tekst", "%" + tekst + "%");
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(ChitatKnigu(reader));
                }
            }

            return spisok;
        }

        public static List<Zhanr> GetZhanryKnigi(int bookId)
        {
            var spisok = new List<Zhanr>();

            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
            SELECT g.GenreId, g.GenreName
            FROM Genres g
            JOIN BookGenres bg ON g.GenreId = bg.GenreId
            WHERE bg.BookId = @bid", soed);

                cmd.Parameters.AddWithValue("@bid", bookId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Zhanr
                    {
                        GenreId = (int)reader["GenreId"],
                        GenreName = reader["GenreName"].ToString()
                    });
                }
            }

            return spisok;
        }
                public static void ZamorozitKnigu(int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Books SET IsFrozen = 1 WHERE BookId = @id", soed);
                cmd.Parameters.AddWithValue("@id", bookId);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Kniga> PoZhanru(int zhanrId)
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.AuthorId, b.IsFrozen,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId
                    JOIN BookGenres bg ON b.BookId = bg.BookId
                    WHERE b.IsFrozen = 0 AND bg.GenreId = @zhanrId
                    GROUP BY b.BookId, b.Title, b.Description,
                             b.CoverPath, b.AuthorId, b.IsFrozen, u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@zhanrId", zhanrId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(ChitatKnigu(reader));
                }
            }

            return spisok;
        }

        public static List<Zhanr> GetVseZhanry()
        {
            var spisok = new List<Zhanr>();

            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT GenreId, GenreName FROM Genres ORDER BY GenreName", soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Zhanr
                    {
                        GenreId = (int)reader["GenreId"],
                        GenreName = reader["GenreName"].ToString()
                    });
                }
            }

            return spisok;
        }

        public static Kniga GetKnigaPoId(int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.AuthorId, b.IsFrozen,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId
                    WHERE b.BookId = @id
                    GROUP BY b.BookId, b.Title, b.Description,
                             b.CoverPath, b.AuthorId, b.IsFrozen, u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@id", bookId);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                    return ChitatKnigu(reader);
            }

            return null;
        }

        private static Kniga ChitatKnigu(SqlDataReader reader)
        {
            return new Kniga
            {
                BookId = (int)reader["BookId"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"] as string,
                CoverPath = reader["CoverPath"] as string,
                AuthorId = (int)reader["AuthorId"],
                AutorImya = reader["AutorImya"].ToString(),
                IsFrozen = (bool)reader["IsFrozen"],
                SrednyayaOtsenka = (double)reader["SredOtsenka"]
            };
        }
        public static string GetNazvanieKnigi(int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT Title FROM Books WHERE BookId = @id", soed);
                cmd.Parameters.AddWithValue("@id", bookId);
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? "Неизвестная книга";
            }
        }
    }
}