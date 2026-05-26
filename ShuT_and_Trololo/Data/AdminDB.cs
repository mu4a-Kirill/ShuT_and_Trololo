using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class AdminDB
    {
        public static List<Polzovatel> GetVsehPolzovateley()
        {
            var spisok = new List<Polzovatel>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT u.UserId, u.Login, u.Email, u.DisplayName,
                           u.RoleId, r.RoleName, u.IsFrozen,
                           u.FreezeReason, u.AvatarPath, u.About
                    FROM Users u
                    JOIN Roles r ON u.RoleId = r.RoleId
                    ORDER BY u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Polzovatel
                    {
                        UserId = (int)reader["UserId"],
                        Login = reader["Login"].ToString(),
                        Email = reader["Email"].ToString(),
                        DisplayName = reader["DisplayName"].ToString(),
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString(),
                        IsFrozen = (bool)reader["IsFrozen"],
                        FreezeReason = reader["FreezeReason"] as string
                    });
                }
            }

            return spisok;
        }

        public static void IzmRol(int userId, int roleId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Users SET RoleId = @rid WHERE UserId = @uid", soed);
                cmd.Parameters.AddWithValue("@rid", roleId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void SmParol(int userId, string novParol)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Users SET Password = @par WHERE UserId = @uid", soed);
                cmd.Parameters.AddWithValue("@par", novParol);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZamorozitPolzovatelya(int userId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Users SET IsFrozen = 1, FreezeReason = @r WHERE UserId = @uid",
                    soed);
                cmd.Parameters.AddWithValue("@r", reason);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void RazmorozitPolzovatelya(int userId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Users SET IsFrozen = 0, FreezeReason = NULL WHERE UserId = @uid",
                    soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Kniga> GetZamorozhKnigi()
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.AuthorId, b.IsFrozen,
                           b.Description, b.CoverPath, b.Content,
                           u.DisplayName AS AutorImya
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    WHERE b.IsFrozen = 1
                    ORDER BY b.Title";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Kniga
                    {
                        BookId = (int)reader["BookId"],
                        Title = reader["Title"].ToString(),
                        AuthorId = (int)reader["AuthorId"],
                        AutorImya = reader["AutorImya"].ToString(),
                        IsFrozen = true
                    });
                }
            }

            return spisok;
        }

        public static List<Otzyv> GetZamorozhOtzyvy()
        {
            var spisok = new List<Otzyv>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT r.ReviewId, r.UserId, r.BookId,
                           r.ReviewText, r.Rating, r.CreatedAt, r.IsFrozen,
                           u.DisplayName AS PolzovatelImya,
                           b.Title AS KnigaTitle
                    FROM Reviews r
                    JOIN Users u ON r.UserId = u.UserId
                    JOIN Books b ON r.BookId = b.BookId
                    WHERE r.IsFrozen = 1
                    ORDER BY r.CreatedAt DESC";

                var cmd = new SqlCommand(sql, soed);
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
                        IsFrozen = true,
                        PolzovatelImya = reader["PolzovatelImya"].ToString()
                    });
                }
            }

            return spisok;
        }

        public static void RazmorozitKnigu(int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Books SET IsFrozen = 0 WHERE BookId = @id", soed);
                cmd.Parameters.AddWithValue("@id", bookId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void RazmorozitOtzyv(int reviewId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Reviews SET IsFrozen = 0 WHERE ReviewId = @id", soed);
                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}