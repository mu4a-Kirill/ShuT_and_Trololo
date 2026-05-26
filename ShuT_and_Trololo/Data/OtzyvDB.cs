using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class OtzyvDB
    {
        public static List<Otzyv> GetOtzyvy(int bookId)
        {
            var spisok = new List<Otzyv>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT r.ReviewId, r.UserId, r.BookId, r.ReviewText,
                           r.Rating, r.CreatedAt, r.IsFrozen,
                           u.DisplayName AS PolzovatelImya
                    FROM Reviews r
                    JOIN Users u ON r.UserId = u.UserId
                    WHERE r.BookId = @bid
                    ORDER BY r.CreatedAt DESC";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@bid", bookId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Otzyv
                    {
                        ReviewId = (int)reader["ReviewId"],
                        UserId = (int)reader["UserId"],
                        BookId = (int)reader["BookId"],
                        ReviewText = reader["ReviewText"] as string,
                        Rating = (int)reader["Rating"],
                        CreatedAt = (System.DateTime)reader["CreatedAt"],
                        IsFrozen = (bool)reader["IsFrozen"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString()
                    });
                }
            }

            return spisok;
        }

        public static List<Otzyv> GetOtzyvyPolzovatelya(int userId)
        {
            var spisok = new List<Otzyv>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT r.ReviewId, r.UserId, r.BookId, r.ReviewText,
                           r.Rating, r.CreatedAt, r.IsFrozen,
                           b.Title AS KnigaTitle,
                           u.DisplayName AS PolzovatelImya
                    FROM Reviews r
                    JOIN Books b ON r.BookId = b.BookId
                    JOIN Users u ON r.UserId = u.UserId
                    WHERE r.UserId = @uid AND r.IsFrozen = 0
                    ORDER BY r.CreatedAt DESC";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var otzyv = new Otzyv
                    {
                        ReviewId = (int)reader["ReviewId"],
                        UserId = (int)reader["UserId"],
                        BookId = (int)reader["BookId"],
                        ReviewText = reader["ReviewText"] as string,
                        Rating = (int)reader["Rating"],
                        CreatedAt = (System.DateTime)reader["CreatedAt"],
                        IsFrozen = (bool)reader["IsFrozen"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString()
                    };
                    spisok.Add(otzyv);
                }
            }

            return spisok;
        }

        public static bool UjeEstOtzyv(int userId, int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Reviews WHERE UserId = @uid AND BookId = @bid",
                    soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static void DobavitOtzyv(int userId, int bookId, string tekst, int rating)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Reviews (UserId, BookId, ReviewText, Rating, IsFrozen)
                    VALUES (@uid, @bid, @tekst, @rating, 0)", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.Parameters.AddWithValue("@tekst", tekst);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZamorozitOtzyv(int reviewId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Reviews SET IsFrozen = 1 WHERE ReviewId = @id", soed);
                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}