using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class ZhalobaDB
    {
        public static List<Zhaloba> GetVseZhaloby()
        {
            var spisok = new List<Zhaloba>();

            using (var soed = BazaDannih.GetSoединение())
            {
                string sql = @"
                    SELECT c.ComplaintId, c.UserId, c.Reason, c.Status,
                           c.BookId, c.ReviewId, c.AuthorId,
                           u.DisplayName AS PolzovatelImya
                    FROM Complaints c
                    JOIN Users u ON c.UserId = u.UserId
                    WHERE c.Status = 'Ожидает'
                    ORDER BY c.ComplaintId DESC";

                var cmd = new SqlCommand(sql, soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spisok.Add(new Zhaloba
                    {
                        ComplaintId = (int)reader["ComplaintId"],
                        UserId = (int)reader["UserId"],
                        PolzovatelImya = reader["PolzovatelImya"].ToString(),
                        Reason = reader["Reason"] as string,
                        BookId = reader["BookId"] as int?,
                        ReviewId = reader["ReviewId"] as int?,
                        AuthorId = reader["AuthorId"] as int?,
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return spisok;
        }

        public static void PrinyatZhalobu(int complaintId, int? bookId,
                                           int? reviewId, int? authorId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd1 = new SqlCommand(
                    "UPDATE Complaints SET Status = 'Принята' WHERE ComplaintId = @id",
                    soed);
                cmd1.Parameters.AddWithValue("@id", complaintId);
                cmd1.ExecuteNonQuery();

                if (bookId.HasValue)
                {
                    var cmd2 = new SqlCommand(
                        "UPDATE Books SET IsFrozen = 1 WHERE BookId = @bid", soed);
                    cmd2.Parameters.AddWithValue("@bid", bookId.Value);
                    cmd2.ExecuteNonQuery();
                }

                if (reviewId.HasValue)
                {
                    var cmd2 = new SqlCommand(
                        "UPDATE Reviews SET IsFrozen = 1 WHERE ReviewId = @rid", soed);
                    cmd2.Parameters.AddWithValue("@rid", reviewId.Value);
                    cmd2.ExecuteNonQuery();
                }

                if (authorId.HasValue)
                {
                    var cmd2 = new SqlCommand(
                        "UPDATE Users SET IsFrozen = 1 WHERE UserId = @uid", soed);
                    cmd2.Parameters.AddWithValue("@uid", authorId.Value);
                    cmd2.ExecuteNonQuery();
                }
            }
        }

        public static void OtklonZhalobu(int complaintId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "UPDATE Complaints SET Status = 'Отклонена' WHERE ComplaintId = @id",
                    soed);
                cmd.Parameters.AddWithValue("@id", complaintId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZhalobaNaKnigu(int userId, int bookId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, BookId, Reason, Status)
                    VALUES (@uid, @bid, @reason, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZhalobaNaAvtora(int userId, int authorId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, AuthorId, Reason, Status)
                    VALUES (@uid, @aid, @reason, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@aid", authorId);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZhalobaNaOtzyv(int userId, int reviewId, string reason)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, ReviewId, Reason, Status)
                    VALUES (@uid, @rid, @reason, 'Ожидает')", soed);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@rid", reviewId);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.ExecuteNonQuery();
            }
        }
    }
}