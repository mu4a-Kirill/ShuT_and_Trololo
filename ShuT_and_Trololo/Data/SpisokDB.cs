using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class SpisokDB
    {
        public static List<(int id, string nazvanie)> GetSektsii()
        {
            var spisok = new List<(int, string)>();

            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT SectionId, SectionName FROM ReadingListSections ORDER BY SectionId",
                    soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add((
                        (int)reader["SectionId"],
                        reader["SectionName"].ToString()
                    ));
                }
            }

            return spisok;
        }

        public static List<Kniga> GetKnigiPolzovatelya(int userId, int sectionId)
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.AuthorId, b.IsFrozen, b.Content,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM ReadingLists rl
                    JOIN Books b ON rl.BookId = b.BookId
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId AND r.IsFrozen = 0
                    WHERE rl.UserId = @uid AND rl.SectionId = @sid
                    GROUP BY b.BookId, b.Title, b.Description, b.CoverPath,
                             b.AuthorId, b.IsFrozen, b.Content, u.DisplayName";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@sid", sectionId);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Kniga
                    {
                        BookId = (int)reader["BookId"],
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"] as string,
                        CoverPath = reader["CoverPath"] as string,
                        Content = reader["Content"] as string,
                        AuthorId = (int)reader["AuthorId"],
                        AutorImya = reader["AutorImya"].ToString(),
                        IsFrozen = (bool)reader["IsFrozen"],
                        SrednyayaOtsenka = (double)reader["SredOtsenka"]
                    });
                }
            }

            return spisok;
        }

        public static void DobavitIliPerenestiKnigu(int userId, int bookId, int sectionId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM ReadingLists WHERE UserId = @uid AND BookId = @bid",
                    soed);
                cmdCheck.Parameters.AddWithValue("@uid", userId);
                cmdCheck.Parameters.AddWithValue("@bid", bookId);
                int count = (int)cmdCheck.ExecuteScalar();

                if (count > 0)
                {
                    var cmdUpd = new SqlCommand(
                        "UPDATE ReadingLists SET SectionId = @sid WHERE UserId = @uid AND BookId = @bid",
                        soed);
                    cmdUpd.Parameters.AddWithValue("@sid", sectionId);
                    cmdUpd.Parameters.AddWithValue("@uid", userId);
                    cmdUpd.Parameters.AddWithValue("@bid", bookId);
                    cmdUpd.ExecuteNonQuery();
                }
                else
                {
                    var cmdIns = new SqlCommand(
                        "INSERT INTO ReadingLists (UserId, BookId, SectionId) VALUES (@uid, @bid, @sid)",
                        soed);
                    cmdIns.Parameters.AddWithValue("@uid", userId);
                    cmdIns.Parameters.AddWithValue("@bid", bookId);
                    cmdIns.Parameters.AddWithValue("@sid", sectionId);
                    cmdIns.ExecuteNonQuery();
                }
            }
        }

        public static void UdalitIzSpiska(int userId, int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "DELETE FROM ReadingLists WHERE UserId = @uid AND BookId = @bid",
                    soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.ExecuteNonQuery();
            }
        }

        public static int GetSektsiyaKnigi(int userId, int bookId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT SectionId FROM ReadingLists WHERE UserId = @uid AND BookId = @bid",
                    soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                var result = cmd.ExecuteScalar();
                return result != null ? (int)result : 0;
            }
        }
    }
}