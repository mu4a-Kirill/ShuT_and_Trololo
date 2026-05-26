using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class AvtorDB
    {
        public static List<Kniga> GetKnigiAvtora(int authorId)
        {
            var spisok = new List<Kniga>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT b.BookId, b.Title, b.Description, b.CoverPath,
                           b.Content, b.AuthorId, b.IsFrozen,
                           u.DisplayName AS AutorImya,
                           ISNULL(AVG(CAST(r.Rating AS FLOAT)), 0) AS SredOtsenka
                    FROM Books b
                    JOIN Users u ON b.AuthorId = u.UserId
                    LEFT JOIN Reviews r ON b.BookId = r.BookId AND r.IsFrozen = 0
                    WHERE b.AuthorId = @aid
                    GROUP BY b.BookId, b.Title, b.Description, b.CoverPath,
                             b.Content, b.AuthorId, b.IsFrozen, u.DisplayName
                    ORDER BY b.Title";

                var cmd = new SqlCommand(sql, soed);
                cmd.Parameters.AddWithValue("@aid", authorId);
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

        public static List<Zhanr> GetVseZhanry()
        {
            return KnigaDB.GetVseZhanry(); 
        }

        public static int DobavitKnigu(string title, string desc,
                                        string coverPath, string content,
                                        int authorId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Books (Title, Description, CoverPath, Content,
                                       AuthorId, IsFrozen)
                    OUTPUT INSERTED.BookId
                    VALUES (@title, @desc, @cover, @content, @aid, 0)", soed);

                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", desc ?? "");
                cmd.Parameters.AddWithValue("@cover", coverPath ?? "");
                cmd.Parameters.AddWithValue("@content", content ?? "");
                cmd.Parameters.AddWithValue("@aid", authorId);

                return (int)cmd.ExecuteScalar();  
            }
        }

        public static void ObnovitKnigu(int bookId, string title, string desc,
                                         string coverPath, string content)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    UPDATE Books
                    SET Title = @title, Description = @desc,
                        CoverPath = @cover, Content = @content
                    WHERE BookId = @id", soed);

                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", desc ?? "");
                cmd.Parameters.AddWithValue("@cover", coverPath ?? "");
                cmd.Parameters.AddWithValue("@content", content ?? "");
                cmd.Parameters.AddWithValue("@id", bookId);

                cmd.ExecuteNonQuery();
            }
        }

        public static void ObnvitZhanry(int bookId, List<int> zhanrIds)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmdDel = new SqlCommand(
                    "DELETE FROM BookGenres WHERE BookId = @bid", soed);
                cmdDel.Parameters.AddWithValue("@bid", bookId);
                cmdDel.ExecuteNonQuery();

                foreach (int zid in zhanrIds)
                {
                    var cmdIns = new SqlCommand(
                        "INSERT INTO BookGenres (BookId, GenreId) VALUES (@bid, @gid)",
                        soed);
                    cmdIns.Parameters.AddWithValue("@bid", bookId);
                    cmdIns.Parameters.AddWithValue("@gid", zid);
                    cmdIns.ExecuteNonQuery();
                }
            }
        }
    }
}