using JsonParser.Formatting;
using System.Text;

namespace JsonParser.JsonStructures
{
    public class JsonValue<T> : JsonNode
    {
        public T Value { get; set; }

        public override string ToString()
        {
            if (typeof(T) == typeof(string))
            {
                return string.Format("\"{0}\"", Value);
            }
            else if (typeof(T) == typeof(bool))
            {
                return Value.ToString().ToLower();
            }
            else
            {
                return Value.ToString();
            }
        }

        public override string ToString(JsonFormat format, JsonNode parentNode, int? baseIndent, int? tabSize)
        {
            var s = ToString();
            if (baseIndent == null)
            {
                return s;
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var c in s)
                {
                    if (c == '\n')
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', tabSize.Value * (baseIndent.Value + 1)));
                    }
                    else if (c != '\r')
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
        }
    }
}
