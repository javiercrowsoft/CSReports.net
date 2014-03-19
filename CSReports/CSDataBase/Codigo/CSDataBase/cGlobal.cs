﻿using System;


namespace CSDataBase
{


    public static class cGlobal
    {
        public static bool Silent = false;
    }

    public enum csDataType
    {
        CSTDCHAR = System.TypeCode.Char,
        CSTDVARCHAR = System.TypeCode.String,
        CSTDLONGVARCHAR = System.TypeCode.String,
        CSTDLONGVARWCHAR = System.TypeCode.String,
        CSTDWCHAR = System.TypeCode.String,
        CSTDVARWCHAR = System.TypeCode.String,
        CSTDDECIMAL = System.TypeCode.String,
        CSTDNUMERIC = System.TypeCode.Decimal,
        CSTDDOUBLE = System.TypeCode.Double,
        CSTDSINGLE = System.TypeCode.Single,
        CSTDCURRENCY = System.TypeCode.Decimal,
        CSTDINTEGER = System.TypeCode.Int32,
        CSTDBIGINT = System.TypeCode.Int64,
        CSTDSMALLINT = System.TypeCode.Int16,
        CSTDTINYINT = System.TypeCode.SByte,
        CSTDUNSIGNEDTINYINT = System.TypeCode.Byte,
        CSTDDBTIME = System.TypeCode.DateTime,
        CSTDDBTIMESTAMP = System.TypeCode.DateTime,
        CSTDDBDATE = System.TypeCode.DateTime,
        CSTDDATE = System.TypeCode.DateTime,
        CSTDBOOLEAN = System.TypeCode.Boolean,
        CSTDBINARY = System.TypeCode.Object,
        CSTDLONGVARBINARY = System.TypeCode.Object
    }

    public enum csCommandType
    {
        CSCMDFILE = 256,
        CSCMDSP = 4,
        CSCMDTABLE = 2,
        CSCMDTABLEDIRECT = 512,
        CSCMDTEXT = 1,
        CSCMDUNKNOWN = -1
    }

}
