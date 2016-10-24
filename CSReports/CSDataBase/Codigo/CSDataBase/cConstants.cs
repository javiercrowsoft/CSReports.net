using System;
using System.Globalization;

namespace CSDataBase
{

    public static class cConstants
    {
        public const string C_SQL_DATE_STRING = "yyyyMMdd HH:mm:ss";
        public const int C_NO_ID = 0;
        public readonly static DateTime C_NO_DATE = DateTime.ParseExact("01/01/1900", "dd/mm/yyyy", CultureInfo.InvariantCulture);
    }
}
