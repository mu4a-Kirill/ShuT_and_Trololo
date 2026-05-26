using System;
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
                    SELECT r.ReviewId, r.UserId, u.DisplayName AS PolzovatelImya,
                           r.BookId, r.ReviewText, r.Rating, r.CreatedAt, r.IsFrozen
                    FROM Reviews r
                    JOIN Users u ON r.UserId = u.UserId
                    WHERE r.BookId = @bookId
                    ORDER BY r.CreatedAt DESC";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@bookId", bookId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Otzyv
                    {
                        ReviewId = (int)reader["ReviewId"],
                        UserId = (int)reader["UserId"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString(),
                        BookId = (int)reader["BookId"],
                        ReviewText = reader["ReviewText"] as string,
                        Rating = (int)reader["Rating"],
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        IsFrozen = (bool)reader["IsFrozen"]
                    });
                }
            }

            return spisok;
        }

        public static void DobavitOtzyv(int userId, int bookId,
                                         string tekst, int rating)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Reviews (UserId, BookId, ReviewText, Rating, CreatedAt)
                    VALUES (@uid, @bid, @tekst, @rating, GETDATE())", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.Parameters.AddWithValue("@tekst", tekst);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool UjeEstOtzyv(int userId, int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Reviews
                    WHERE UserId = @uid AND BookId = @bid", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static void ZamorozitOtzyv(int reviewId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    UPDATE Reviews SET IsFrozen = 1
                    WHERE ReviewId = @id", soed);

                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}