using System;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class PolzovatelDB
    {
        public static Polzovatel Voiti(string login, string parol)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"SELECT u.UserId, u.Login, u.Password, u.Email,
                               u.DisplayName, u.RoleId, r.RoleName,
                               u.IsFrozen, u.FreezeReason, u.AvatarPath, u.About
                               FROM Users u
                               JOIN Roles r ON u.RoleId = r.RoleId
                               WHERE u.Login = @login AND u.Password = @parol";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@parol", parol);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Polzovatel
                    {
                        UserId = (int)reader["UserId"],
                        Login = reader["Login"].ToString(),
                        Email = reader["Email"].ToString(),
                        DisplayName = reader["DisplayName"].ToString(),
                        RoleId = (int)reader["RoleId"],
                        RoleName = reader["RoleName"].ToString(),
                        IsFrozen = (bool)reader["IsFrozen"],
                        FreezeReason = reader["FreezeReason"] as string,
                        AvatarPath = reader["AvatarPath"] as string,
                        About = reader["About"] as string
                    };
                }
                return null; 
            }
        }

        public static bool LoginZanyat(string login)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Login = @login", soed);
                cmd.Parameters.AddWithValue("@login", login);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static bool EmailZanyat(string email)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email = @email", soed);
                cmd.Parameters.AddWithValue("@email", email);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static void Zaregistrirovat(string login, string parol,
                                            string email, string imya)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Users (Login, Password, Email, DisplayName, RoleId, IsFrozen)
                    VALUES (@login, @parol, @email, @imya, 1, 0)", soed);

                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@parol", parol);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@imya", imya);
                cmd.ExecuteNonQuery();
            }
        }
    }
}