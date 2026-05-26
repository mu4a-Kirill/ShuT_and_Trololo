using ShuT_and_Trololo.Models;

namespace ShuT_and_Trololo.Data
{
    public static class Sessiya
    {
        public static Polzovatel TekushiyPolzovatel { get; set; }

        public static bool EstPolzovatel => TekushiyPolzovatel != null;

        public static bool EtoAdmin =>
            TekushiyPolzovatel?.RoleName == "Администратор";

        public static bool EtoAvtor =>
            TekushiyPolzovatel?.RoleName == "Автор";

        public static bool AkkZamorozhen =>
            TekushiyPolzovatel?.IsFrozen == true;

        public static void Viyti()
        {
            TekushiyPolzovatel = null;
        }
    }
}