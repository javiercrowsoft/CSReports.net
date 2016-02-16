using System;


namespace CSDataBase
{
    //
    // TODO: has a function to translate from Ado.net to csAdoDataType which is the value
    //       we use to save in csr files
    //
    //       use the table here http://www.frentonline.com/Knowledgebase/MSSQLServer/Datatype/tabid/362/Default.aspx
    //

    public static class cDatabaseGlobals
    {
        public static bool Silent = false;

        public static bool isNumberField(int fieldType)
        {
            switch ((csAdoDataType)fieldType) 
            { 
                case csAdoDataType.adDecimal:
                case csAdoDataType.adDouble: 
                case csAdoDataType.adInteger:
                case csAdoDataType.adCurrency: 
                case csAdoDataType.adBigInt: 
                case csAdoDataType.adNumeric:
                case csAdoDataType.adSingle:
                case csAdoDataType.adSmallInt:
                case csAdoDataType.adTinyInt:
                case csAdoDataType.adUnsignedBigInt:
                case csAdoDataType.adUnsignedInt:
                case csAdoDataType.adUnsignedSmallInt:
                case csAdoDataType.adUnsignedTinyInt:
                case csAdoDataType.adVarNumeric:
                    return true;
            }
            return false;
        }
    }

    public enum csDataType
    {
        CSTDCHAR = System.TypeCode.Char,
        CSTDVARCHAR = System.TypeCode.String,
        CSTDLONGVARCHAR = System.TypeCode.String,
        CSTDLONGVARWCHAR = System.TypeCode.String,
        CSTDWCHAR = System.TypeCode.String,
        CSTDVARWCHAR = System.TypeCode.String,
        CSTDDECIMAL = System.TypeCode.Decimal,
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

    public enum csAdoDataType {
        adBigInt = 20,
        adBinary = 128,
        adBoolean = 11,
        adBSTR = 8,
        adChapter = 136,
        adChar = 129,
        adCurrency = 6,
        adDate = 7,
        adDBDate = 133,
        adDBFileTime = 137,
        adDBTime = 134,
        adDBTimeStamp = 135,
        adDecimal = 14,
        adDouble = 5,
        adEmpty = 0,
        adError = 10,
        adFileTime = 64,
        adGUID = 72,
        adIDispatch = 9,
        adInteger = 3,
        adIUnknown = 13,
        adLongVarBinary = 205,
        adLongVarChar = 201,
        adLongVarWChar = 203,
        adNumeric = 131,
        adPropVariant = 138,
        adSingle = 4,
        adSmallInt = 2,
        adTinyInt = 16,
        adUnsignedBigInt = 21,
        adUnsignedInt = 19,
        adUnsignedSmallInt = 18,
        adUnsignedTinyInt = 17,
        adUserDefined = 132,
        adVarBinary = 204,
        adVarChar = 200,
        adVariant = 12,
        adVarNumeric = 139,
        adVarWChar = 202,
        adWChar = 130
    }

}
