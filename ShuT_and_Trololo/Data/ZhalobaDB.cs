using Microsoft.Data.SqlClient;

namespace ShuT_and_Trololo.Data
{
    public static class ZhalobaDB
    {
        public static void ZhalobaNaKnigu(int userId, int bookId, string prichina)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, Reason, BookId, Status)
                    VALUES (@uid, @reason, @bid, 'Ожидает')", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@reason", prichina);
                cmd.Parameters.AddWithValue("@bid", bookId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZhalobaNaOtzyv(int userId, int reviewId, string prichina)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, Reason, ReviewId, Status)
                    VALUES (@uid, @reason, @rid, 'Ожидает')", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@reason", prichina);
                cmd.Parameters.AddWithValue("@rid", reviewId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ZhalobaNaAvtora(int userId, int authorId, string prichina)
        {
            using (var soed = BazaDannih.GetSoединение())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Complaints (UserId, Reason, AuthorId, Status)
                    VALUES (@uid, @reason, @aid, 'Ожидает')", soed);

                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@reason", prichina);
                cmd.Parameters.AddWithValue("@aid", authorId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}