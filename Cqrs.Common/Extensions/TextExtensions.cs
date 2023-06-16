using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cqrs.Common.Extensions
{
    public static class TextExtensions
    {

        public static string Pluralize(this string name)
        {
            if (name.EndsWith("child", StringComparison.InvariantCultureIgnoreCase)) return name + "ren";
            if (name.EqualsNoCase("person")) return "People";
            if (Regex.IsMatch(name, @"\w+[aeiouAEIOU]{1}y$", RegexOptions.IgnoreCase)) return name + "s";
            if (name.EndsWith("ss")) return name + "es";
            return name.EndsWith("y") ? name.Substring(0, name.Length - 1) + "ies" : name + "s";
        }

        public static string Singularize(this string name)
        {
            if (name.EndsWith("children", StringComparison.InvariantCultureIgnoreCase)) return Regex.Replace(name, "ren$", string.Empty);
            if (Regex.IsMatch(name, @"ies$", RegexOptions.IgnoreCase)) return Regex.Replace(name, "ies$", "y");
            if (Regex.IsMatch(name, @"sses$", RegexOptions.IgnoreCase)) return Regex.Replace(name, "sses$", "ss");
            return name.EndsWith("s") ? name.Substring(0, name.Length - 1) : name;
        }

        public static string ToCamelCase(this string name)
        {
            if (name.Length == 0) return name;
            if (name.ToLower() == "id") return "id";
            return name[0].ToString().ToLower() + name.Substring(1);
        }

        public static bool EqualsNoCase(this string name, string compare)
        {
            return name.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsCase(this string source, string toCheck, StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string CsvEscape(this string data, string delimiter = ",")
        {
            if(string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            string pattern = "\\\"|\\'|\\r|\\n";
            if (!(string.IsNullOrEmpty(delimiter)))
            {
                pattern += "|\\,";
            }
            else
            {
                pattern += "|\\" + delimiter;
            }

            string output = string.Empty;
            if (Regex.IsMatch(data, pattern))
            {
                output = "\"" + data.Replace("\"", "\"\"") + "\"";
            }
            else
            {
                output = data;
            }
            return output;
        }
        /// <summary>
        /// Interpolates a string with values, based on named properties.
        /// </summary>
        /// <param name="formatString">The format string, uses the {propName} pattern.</param>
        /// <param name="obj">The object containing the property values.</param>
        /// <remarks>Use with caution because this method uses reflection to get the values of the named properties of the T object.
        /// The method requires an object and its properties to identify the names in the format string. 
        /// Local variables and parameters don't work with names, only with indexes.
        /// Example:
        /// string format = "{Name} has {AppleCount} apples.";
        /// var o = new { Name = "Anna", AppleCount = 3 };
        /// string output = format.Interpolate(o); //Anna has 3 apples.
        /// </remarks>
        public static string Interpolate<T>(this string formatString, T obj)
        {
            return formatString.Interpolate(obj, typeof(T));
        }

        public static string Interpolate(this string formatString, object obj, Type type)
        {
            if (obj == null)
            {
                return formatString;
            }
            string retString = formatString;
            object oValue = null;
            string value = null;
            Int32 index = 0;
            foreach (var p in type.GetProperties())
            {
                oValue = p.GetValue(obj, null);
                if (oValue == null)
                {
                    value = string.Empty;
                }
                else
                {
                    value = oValue.ToString();
                }
                retString = Regex.Replace(retString, "\\{" + p.Name + "\\}", value, RegexOptions.IgnoreCase);
                index += 1;
            }
            return retString;
        }

        public static string MakeIdentifier(this string value, string badCharReplacement = "_")
        {
            if(string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            string pattern = @"\W";
            string val = Regex.Replace(value, pattern, badCharReplacement ?? "", RegexOptions.IgnoreCase);
            //if(!Validator.IsIdentifier(val))
            //{
            //    val = "_" + val;
            //}
            return val;

        }

        public static string SplitCamelCase(this string input, string separator = " ")
        {
            return Regex.Replace(input, "(?<=[a-z])([A-Z])", separator + "$1", RegexOptions.Compiled).Trim();
        }

        public static string SplitLowerCase(this string input, string separator = " ")
        {
            return input.SplitCamelCase(separator).ToLowerInvariant();
        }

        public static string SplitUpperCase(this string input, string separator = " ")
        {
            return input.SplitCamelCase(separator).ToUpperInvariant();
        }

        public static string ReplaceNoCase(this string input, string search, string replacement)
        {
            string result = Regex.Replace(input, search, replacement, RegexOptions.IgnoreCase);
            return result;
        }

        public static string NormalizeKey(this string key, char badCharReplacement = '-')
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            var pattern = @"[\W-[\" + badCharReplacement.ToString() + "]]";
            var normKey = Regex.Replace(key, pattern, badCharReplacement.ToString());
            return normKey;
        }
        public static Stream AsStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string AsString(this Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
