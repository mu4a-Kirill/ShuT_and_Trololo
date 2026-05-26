using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class ZayavkaDB
    {
        public static bool EstZayavkaNaAvtora(int userId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM RoleApplications WHERE UserId = @uid AND Status = 'Ожидает'",
                    soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static void PodatZayavkuNaAvtora(int userId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO RoleApplications (UserId, Status)
                    VALUES (@uid, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void PodatZayavkuNaRazmorozku(int userId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO UnfreezeApplications (UserId, BookId, Reason, Status)
                    VALUES (@uid, NULL, @reason, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.ExecuteNonQuery();
            }
        }

        public static void PodatZayavkuNaRazmorzKnigu(int userId, int bookId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO UnfreezeApplications (UserId, BookId, Reason, Status)
                    VALUES (@uid, @bid, @reason, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Zayavka> GetZayavkiNaAvtora()
        {
            var spisok = new List<Zayavka>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT ra.ApplicationId, ra.UserId, ra.Status, ra.CreatedAt,
                           u.DisplayName AS PolzovatelImya
                    FROM RoleApplications ra
                    JOIN Users u ON ra.UserId = u.UserId
                    WHERE ra.Status = 'Ожидает'
                    ORDER BY ra.CreatedAt";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Zayavka
                    {
                        ApplicationId = (int)reader["ApplicationId"],
                        UserId = (int)reader["UserId"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString(),
                        Status = reader["Status"].ToString(),
                        CreatedAt = (System.DateTime)reader["CreatedAt"]
                    });
                }
            }

            return spisok;
        }

        public static List<Zayavka> GetZayavkiNaRazmorozku()
        {
            var spisok = new List<Zayavka>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT ua.ApplicationId, ua.UserId, ua.BookId, ua.Reason,
                           ua.Status, ua.CreatedAt,
                           u.DisplayName AS PolzovatelImya,
                           b.Title AS KnigaTitle
                    FROM UnfreezeApplications ua
                    JOIN Users u ON ua.UserId = u.UserId
                    LEFT JOIN Books b ON ua.BookId = b.BookId
                    WHERE ua.Status = 'Ожидает'
                    ORDER BY ua.CreatedAt";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Zayavka
                    {
                        ApplicationId = (int)reader["ApplicationId"],
                        UserId = (int)reader["UserId"],
                        BookId = reader["BookId"] as int?,
                        Reason = reader["Reason"] as string,
                        Status = reader["Status"].ToString(),
                        CreatedAt = (System.DateTime)reader["CreatedAt"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString(),
                        KnigaTitle = reader["KnigaTitle"] as string
                    });
                }
            }

            return spisok;
        }

        public static void PrinyatZayavkuAvtora(int applicationId, int userId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd1 = new SqlCommand(
                    "UPDATE RoleApplications SET Status = 'Принята' WHERE ApplicationId = @id",
                    soed);
                cmd1.Parameters.AddWithValue("@id", applicationId);
                cmd1.ExecuteNonQuery();

                var cmd2 = new SqlCommand(
                    "UPDATE Users SET RoleId = 2 WHERE UserId = @uid", soed);
                cmd2.Parameters.AddWithValue("@uid", userId);
                cmd2.ExecuteNonQuery();
            }
        }

        public static void OtklonZayavkuAvtora(int applicationId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE RoleApplications SET Status = 'Отклонена' WHERE ApplicationId = @id",
                    soed);
                cmd.Parameters.AddWithValue("@id", applicationId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void PrinyatRazmorozku(int applicationId, int userId, int? bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd1 = new SqlCommand(
                    "UPDATE UnfreezeApplications SET Status = 'Принята' WHERE ApplicationId = @id",
                    soed);
                cmd1.Parameters.AddWithValue("@id", applicationId);
                cmd1.ExecuteNonQuery();

                if (bookId.HasValue)
                {
                    var cmd2 = new SqlCommand(
                        "UPDATE Books SET IsFrozen = 0 WHERE BookId = @bid", soed);
                    cmd2.Parameters.AddWithValue("@bid", bookId.Value);
                    cmd2.ExecuteNonQuery();
                }
                else
                {
                    var cmd2 = new SqlCommand(
                        "UPDATE Users SET IsFrozen = 0, FreezeReason = NULL WHERE UserId = @uid",
                        soed);
                    cmd2.Parameters.AddWithValue("@uid", userId);
                    cmd2.ExecuteNonQuery();
                }
            }
        }

        public static void OtklonRazmorozku(int applicationId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE UnfreezeApplications SET Status = 'Отклонена' WHERE ApplicationId = @id",
                    soed);
                cmd.Parameters.AddWithValue("@id", applicationId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}