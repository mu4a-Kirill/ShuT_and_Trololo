using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;

namespace ShuT_and_Trololo.Data
{
    public static class BazaDannih
    {
        private static string strPodkluch = ConfigurationManager
            .ConnectionStrings["BD_ShuT"].ConnectionString;

        public static SqlConnection GetSoединение()
        {
            var soed = new SqlConnection(strPodkluch);
            soed.Open();
            return soed;
        }
    }
}