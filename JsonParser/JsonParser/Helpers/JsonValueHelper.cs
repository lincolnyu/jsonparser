using System;
using System.Collections.Generic;
using System.Text;
using JsonParser.JsonStructures;

namespace JsonParser.Helpers
{
    public static class JsonValueHelper
    {
        /// <summary>
        ///  de-escape the string
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="inQuote">
        ///  The  predicate provided by the caller that tells this method whether 
        ///  currently the string 
        /// </param>
        /// <param name="start">Optional start position (inclusive)</param>
        /// <param name="end">Optional end position (esclusive)</param>
        /// <returns>
        ///  A enumerable of tuples that give
        ///   1. The de-escaped original character (vertabim)
        ///   2. The index of the character in the string (excluding the escape sign)
        ///   3. Whether this is de-escaped character or character as is.
        /// </returns>
        public static IEnumerable<Tuple<char, int, bool>> DeEscape(this string s, Func<bool> inQuote, int start = 0, int? end = null)
        {
            var endPos = end ?? s.Length;
            for (var i = start; i < endPos; i++)
            {
                var c = s[i];
                var escaped = false;
                if (c == '\\' && inQuote())
                {
                    if (++i >= s.Length)
                    {
                        break;
                    }
                    c = s[i];
                    escaped = true;
                }
                yield return new Tuple<char, int, bool>(c, i, escaped);
            }
        }

        public static string ParseQuote(this string s)
        {
            var sb = new StringBuilder();
            foreach (var t in DeEscape(s, ()=>true))
            {
                sb.Append(t.Item1);
            }
            return sb.ToString();
        }

        public static string GetJsonKey(this string s)
        {
            s = s.Trim();
            if (s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                s = s.Substring(1, s.Length - 2).ParseQuote();
            }
            return s;
        }

        public static JsonNode GetJsonValue(this string s)
        {
            s = s.Trim();
            if (s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                var jsonString = new JsonValue<string>()
                {
                    Value = s.Substring(1, s.Length - 2).ParseQuote()
                };
                return jsonString;
            }
            else
            {
                if (bool.TryParse(s, out var bval))
                {
                    return new JsonValue<bool>
                    {
                        Value = bval
                    };
                }
                else if (Numeric.IsDecimalNumeric(s))
                {
                    return new JsonValue<Numeric>
                    {
                        Value = new Numeric(s)
                    };
                }
                else if (int.TryParse(s, out var ival))
                {
                    return new JsonValue<int>
                    {
                        Value = ival
                    };
                }
                if (double.TryParse(s, out var fval))
                {
                    return new JsonValue<double>
                    {
                        Value = fval
                    };
                }
                else
                {
                    // as string
                    var jsonString = new JsonValue<string>()
                    {
                        Value = s
                    };
                    return jsonString;
                }
                // TODO other types?
            }
        }
    }
}
