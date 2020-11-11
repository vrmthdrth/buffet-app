using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetAuxiliaryLib.BLL
{
    public class ArraysConvert
    {
        public static decimal[] FromStringToDecimal(string[] stringArray)
        {
            int length = stringArray.Length;
            decimal[] decimalArray = new decimal[length];
            for (int i = 0; i < length; i++)
            {
                decimalArray[i] = decimal.Parse(stringArray[i], CultureInfo.InvariantCulture);
            }
            return decimalArray;
        }

        public static int?[] FromStringToNullableInt(string[] stringArray)
        {
            int length = stringArray.Length;
            int?[] nullableIntArray = new int?[length];
            for (int i = 0; i < length; i++)
            {
                int outInt;
                nullableIntArray[i] = int.TryParse(stringArray[i], out outInt) ? (int?)outInt : null;
            }
            return nullableIntArray;
        }

        public static int[] FromStringToInt(string[] stringArray)
        {
            int length = stringArray.Length;
            int[] intArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                intArray[i] = int.Parse(stringArray[i]);
            }
            return intArray;
        }
    }
}
