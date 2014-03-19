using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CSKernelClient
{
    public static class G
    {
        public static bool isNumeric(string value)
        {
            double dummyNumber;
            return Double.TryParse(value, out dummyNumber);
        }

        // bytes

        public static void redim(ref byte[] vBytes, int size)
        {
            if (size == 0)
            {
                vBytes = null;
            }
            else
            {
                vBytes = new byte[size];
            }
        }

        public static void redimPreserve(ref byte[] vBytes, int size) 
        {
            if (size == 0)
            {
                vBytes = null;
            }
            else
            {
                if (vBytes == null)
                {
                    vBytes = new byte[size];
                }
                else if (vBytes.Length == 0)
                {
                    vBytes = new byte[size];
                }
                else
                {
                    byte[] newArray = new byte[size];
                    Array.Copy(vBytes, newArray, vBytes.Length);
                    vBytes = newArray;
                }
            }
        }

        // objects

        public static void redim(ref object[] vObjects, int size)
        {
            if (size == 0)
            {
                vObjects = null;
            }
            else
            {
                vObjects = new object[size];
            }
        }

        public static void redimPreserve(ref object[] objects, int size)
        {
            if (size == 0)
            {
                objects = null;
            }
            else
            {
                if (objects == null)
                {
                    objects = new object[size];
                }
                else if (objects.Length == 0)
                {
                    objects = new object[size];
                }
                else
                {
                    object[] newArray = new object[size];
                    Array.Copy(objects, newArray, objects.Length);
                    objects = newArray;
                }
            }
        }

        // strings

        public static void redim(ref String[] vStrings, int size)
        {
            if (size == 0)
            {
                vStrings = null;
            }
            else
            {
                vStrings = new String[size];
            }
        }

        public static void redim(ref String[,] vStrings, int size1, int size2)
        {
            if (size1 == 0)
            {
                vStrings = null;
            }
            else
            {
                vStrings = new String[size1, size2];
            }
        }

        public static void redimPreserve(ref String[] vStrings, int size)
        {
            if (size == 0)
            {
                vStrings = null;
            }
            else
            {
                if (vStrings == null)
                {
                    vStrings = new String[size];
                }
                else if (vStrings.Length == 0)
                {
                    vStrings = new String[size];
                }
                else
                {
                    String[] newArray = new String[size];
                    Array.Copy(vStrings, newArray, vStrings.Length);
                    vStrings = newArray;
                }
            }
        }

        // ints

        public static void redim(ref int[] vInts, int size)
        {
            if (size == 0)
            {
                vInts = null;
            }
            else
            {
                vInts = new int[size];
            }
        }

        public static void redimPreserve(ref int[] vInts, int size)
        {
            if (size == 0)
            {
                vInts = null;
            }
            else
            {
                if (vInts == null)
                {
                    vInts = new int[size];
                }
                else if (vInts.Length == 0)
                {
                    vInts = new int[size];
                }
                else
                {
                    int[] newArray = new int[size];
                    Array.Copy(vInts, newArray, vInts.Length);
                    vInts = newArray;
                }
            }
        }

        // floats

        public static void redim(ref float[] vFloats, int size)
        {
            if (size == 0)
            {
                vFloats = null;
            }
            else
            {
                vFloats = new float[size];
            }
        }

        public static void redimPreserve(ref float[] vFloats, int size)
        {
            if (size == 0)
            {
                vFloats = null;
            }
            else
            {
                if (vFloats == null)
                {
                    vFloats = new float[size];
                }
                else if (vFloats.Length == 0)
                {
                    vFloats = new float[size];
                }
                else
                {
                    float[] newArray = new float[size];
                    Array.Copy(vFloats, newArray, vFloats.Length);
                    vFloats = newArray;
                }
            }
        }

        // DataTables

        public static void redim(ref DataTable[] vDataTables, int size)
        {
            if (size == 0)
            {
                vDataTables = null;
            }
            else
            { 
                vDataTables = new DataTable[size];
            }
        }

        public static void redimPreserve(ref DataTable[] vDataTables, int size)
        {
            if (size == 0)
            {
                vDataTables = null;
            }
            else
            {
                if (vDataTables == null)
                {
                    vDataTables = new DataTable[size];
                }
                else if (vDataTables.Length == 0)
                {
                    vDataTables = new DataTable[size];
                }
                else
                {
                    DataTable[] newArray = new DataTable[size];
                    Array.Copy(vDataTables, newArray, vDataTables.Length);
                    vDataTables = newArray;
                }
            }
        }

    }

}
