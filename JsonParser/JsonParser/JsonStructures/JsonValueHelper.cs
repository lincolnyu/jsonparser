using System;
using System.Collections.Generic;
using System.Text;

namespace JsonParser.JsonStructures
{
    public static class JsonValueHelper
    {
        public static IEnumerable<Tuple<char, int>> DeEscape(this string s, Func<bool> inQuote)
        {
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '\\' && inQuote())
                {
                    if (++i >= s.Length)
                    {
                        break;
                    }
                    c = s[i];
                }
                yield return new Tuple<char, int>(c, i);
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
                if (int.TryParse(s, out int ival))
                {
                    var jsonInt = new JsonValue<int>
                    {
                        Value = ival
                    };
                    return jsonInt;
                }
                else if (bool.TryParse(s, out bool bval))
                {
                    var jsonBool = new JsonValue<bool>
                    {
                        Value = bval
                    };
                    return jsonBool;
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
