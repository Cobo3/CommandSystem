/*This File provides various Parser methods used for some common Types*/
namespace SickDev.CommandSystem 
{
    static class Parsers 
    {
        const string nullObject = "null";

        [Parser(typeof(object))]
        static object ParseObject(string value) => ParseString(value);

        [Parser(typeof(string))]
        static string ParseString(string value) => value.Equals(nullObject) ? null : value;

        [Parser(typeof(byte))]
        static byte ParseByte(string value) => byte.Parse(value.Trim());

        [Parser(typeof(byte?))]
        static byte? ParseNullableByte(string value) => value.Equals(nullObject) ? (byte?)null : ParseByte(value);

        [Parser(typeof(sbyte))]
        static sbyte ParseSbyte(string value) => sbyte.Parse(value.Trim());

        [Parser(typeof(sbyte?))]
        static sbyte? ParseNullableSbyte(string value) => value.Equals(nullObject) ? (sbyte?)null : ParseSbyte(value);

        [Parser(typeof(short))]
        static short ParseShort(string value) => short.Parse(value.Trim());

        [Parser(typeof(short?))]
        static short? ParseNullableShort(string value) => value.Equals(nullObject) ? (short?)null : ParseShort(value);

        [Parser(typeof(ushort))]
        static ushort ParseUshort(string value) => ushort.Parse(value.Trim());

        [Parser(typeof(ushort?))]
        static ushort? ParseNullableUshort(string value) => value.Equals(nullObject) ? (ushort?)null : ParseUshort(value);

        [Parser(typeof(int))]
        static int ParseInt(string value) => int.Parse(value.Trim());

        [Parser(typeof(int?))]
        static int? ParseNullableInt(string value) => value.Equals(nullObject) ? (int?)null : ParseInt(value);

        [Parser(typeof(uint))]
        static uint ParseUint(string value) => uint.Parse(value.Trim());

        [Parser(typeof(uint?))]
        static uint? ParseNullableUint(string value) => value.Equals(nullObject) ? (uint?)null : ParseUint(value);

        [Parser(typeof(long))]
        static long ParseLong(string value) => long.Parse(value.Trim());

        [Parser(typeof(long?))]
        static long? ParseNullableLong(string value) => value.Equals(nullObject) ? (long?)null : ParseLong(value);

        [Parser(typeof(ulong))]
        static ulong ParseUlong(string value) => ulong.Parse(value.Trim());

        [Parser(typeof(ulong?))]
        static ulong? ParseNullableUlong(string value) => value.Equals(nullObject) ? (ulong?)null : ParseUlong(value);

        [Parser(typeof(float))]
        static float ParseFloat(string value) => float.Parse(value.Trim());

        [Parser(typeof(float?))]
        static float? ParseNullableFloat(string value) => value.Equals(nullObject) ? (float?)null : ParseFloat(value);

        [Parser(typeof(double))]
        static double ParseDouble(string value) => double.Parse(value.Trim());

        [Parser(typeof(double?))]
        static double? ParseNullableDouble(string value) => value.Equals(nullObject) ? (double?)null : ParseDouble(value);

        [Parser(typeof(decimal))]
        static decimal ParseDecimal(string value) => decimal.Parse(value.Trim());

        [Parser(typeof(decimal?))]
        static decimal? ParseNullableDecimal(string value) => value.Equals(nullObject) ? (decimal?)null : ParseDecimal(value);

        //TRUE-->	[true, 1, yes, y, t]
        //FALSE-->	[false, 0, no, n, f]
        [Parser(typeof(bool))]
        static bool ParseBool(string value) 
        {
            bool bResult = false;
            if(bool.TryParse(value, out bResult))
                return bResult;
            else 
            {
                int iResult;
                if(int.TryParse(value, out iResult)) 
                {
                    if(iResult == 1)
                        return true;
                    else if(iResult == 0)
                        return false;
                    else
                        throw new InvalidArgumentFormatException<bool>(value);
                }
                else 
                {
                    if(value.Equals("yes") || value.Equals("y") || value.Equals("t"))
                        return true;
                    else if(value.Equals("no") || value.Equals("n") || value.Equals("f"))
                        return false;
                    else
                        throw new InvalidArgumentFormatException<bool>(value);
                }
            }
        }

        [Parser(typeof(bool?))]
        static bool? ParseNullableBool(string value) => value.Equals(nullObject) ? (bool?)null : ParseBool(value);

        [Parser(typeof(char))]
        static char ParseChar(string value) => char.Parse(value);

        [Parser(typeof(char?))]
        static char? ParseNullableChar(string value) => value.Equals(nullObject) ? (char?)null : ParseChar(value);
    }
}