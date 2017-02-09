/*This File provides various Parser methods used for some common Types*/
namespace SickDev.CommandSystem {
    internal static partial class Parsers {

        const string nullObject = "null";
        /**********************************************
		* C# primitive types
		**********************************************/
        [Parser(typeof(string))]
        static string ParseString(string value) {
            return value.Equals(nullObject) ? null : value;
        }

        //TRUE-->	[true, 1, yes, y, t]
        //FALSE-->	[false, 0, no, n, f]
        [Parser(typeof(bool))]
        static bool ParseBool(string value) {
            bool bResult = false;
            if (bool.TryParse(value, out bResult))
                return bResult;
            else {
                int iResult;
                if (int.TryParse(value, out iResult)) {
                    if (iResult == 1)
                        return true;
                    else if (iResult == 0)
                        return false;
                    else
                        throw new InvalidArgumentFormatException<bool>(value);
                }
                else {
                    if (value.Equals("yes") || value.Equals("y") || value.Equals("t"))
                        return true;
                    else if (value.Equals("no") || value.Equals("n") || value.Equals("f"))
                        return false;
                    else
                        throw new InvalidArgumentFormatException<bool>(value);
                }
            }
        }

        [Parser(typeof(bool?))]
        static bool? ParseNullableBool(string value) {
            try {
                return value.Equals(nullObject) ? (bool?)null : ParseBool(value);
            }
            catch (CommandSystemException e) {
                throw e;
            }
        }

        [Parser(typeof(int))]
        static int ParseInt(string value) {
            try {
                return int.Parse(value.Trim());
            }
            catch {
                throw new InvalidArgumentFormatException<int>(value);
            }
        }

        [Parser(typeof(int?))]
        static int? ParseNullableInt(string value) {
            try {
                return value.Equals(nullObject) ? (int?)null : ParseInt(value);
            }
            catch (CommandSystemException e) {
                throw e;
            }
        }

        [Parser(typeof(float))]
        static float ParseFloat(string value) {
            try {
                return float.Parse(value.Trim());
            }
            catch {
                throw new InvalidArgumentFormatException<float>(value);
            }
        }

        [Parser(typeof(float?))]
        static float? ParseNullableFloat(string value) {
            try {
                return value.Equals(nullObject) ? (float?)null : ParseFloat(value);
            }
            catch (CommandSystemException e) {
                throw e;
            }
        }

        [Parser(typeof(char))]
        static char ParseChar(string value) {
            try {
                return char.Parse(value);
            }
            catch {
                throw new InvalidArgumentFormatException<char>(value);
            }
        }

        [Parser(typeof(char?))]
        static char? ParseNullableChar(string value) {
            try {
                return value.Equals(nullObject) ? (char?)null : ParseChar(value);
            }
            catch (CommandSystemException e) {
                throw e;
            }
        }
    }
}