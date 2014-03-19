using System;


namespace CSKernelClient
{


    public static class cDateUtils
    {
        public static bool isDate(object dateValue)
        {
            Type t = dateValue.GetType();
            if (typeof(DateTime) == t)
            {
                return true;
            }
            else
            {
                if (typeof(string) == t)
                {
                    try
                    {
                        DateTime.Parse((string)dateValue);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
