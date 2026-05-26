using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ShuT_and_Trololo.Data
{
    public static class SpisokDB
    {
        public static void DobavitIliPerenestiKnigu(int userId, int bookId, int sectionId)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmdCheck = new SqlCommand(@"
                    SELECT COUNT(*) FROM ReadingLists
                    WHERE UserId = @uid AND BookId = @bid", soed);
                cmdCheck.Parameters.AddWithValue("@uid", userId);
                cmdCheck.Parameters.AddWithValue("@bid", bookId);
                bool ujeEst = (int)cmdCheck.ExecuteScalar() > 0;

                if (ujeEst)
                {
                    var cmdUpd = new SqlCommand(@"
                        UPDATE ReadingLists SET SectionId = @sid
                        WHERE UserId = @uid AND BookId = @bid", soed);
                    cmdUpd.Parameters.AddWithValue("@sid", sectionId);
                    cmdUpd.Parameters.AddWithValue("@uid", userId);
                    cmdUpd.Parameters.AddWithValue("@bid", bookId);
                    cmdUpd.ExecuteNonQuery();
                }
                else
                {
                    var cmdIns = new SqlCommand(@"
                        INSERT INTO ReadingLists (UserId, BookId, SectionId)
                        VALUES (@uid, @bid, @sid)", soed);
                    cmdIns.Parameters.AddWithValue("@uid", userId);
                    cmdIns.Parameters.AddWithValue("@bid", bookId);
                    cmdIns.Parameters.AddWithValue("@sid", sectionId);
                    cmdIns.ExecuteNonQuery();
                }
            }
        }

        public static List<(int Id, string Nazvanie)> GetSektsii()
        {
            var list = new List<(int, string)>();

            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(
                    "SELECT SectionId, SectionName FROM ReadingListSections", soed);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                    list.Add(((int)reader["SectionId"], reader["SectionName"].ToString()));
            }

            return list;
        }
    }
}
