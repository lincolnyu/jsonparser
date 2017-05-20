using System.Collections.Generic;
using System.Text;
using JsonParser.Helpers;

namespace JsonParser.JsonStructures
{
    public class JsonArray : JsonNode
    {
        public List<JsonNode> Items { get; private set; } = new List<JsonNode>();

        public override string ToString() => ToString(null, null);

        public override string ToString(int? baseIndent, int? tabSize)
        {
            if (Items.Count == 0)
            {
                return "[]";
            }
            var sb = new StringBuilder();
            sb.Append('[');
            if (baseIndent != null)
            {
                sb.AppendLine();
            }
            foreach (var item in Items)
            {
                if (baseIndent != null)
                {
                    sb.Append(new string(' ', tabSize.Value * (baseIndent.Value + 1)));
                }
                if (item is JsonPairs jsonPairs)
                {
                    sb.Append(jsonPairs.ToNakedStringIfPossible((baseIndent + 1) ?? null, tabSize));
                }
                else
                {
                    sb.Append(item.ToString((baseIndent + 1) ?? null, tabSize));
                }
                sb.Append(',');
                if (baseIndent != null)
                {
                    sb.AppendLine();
                }
            }
            var comma = sb.ToString().LastIndexOf(",");
            sb.Remove(comma, sb.Length - comma); // remove the last comma
            if (baseIndent != null)
            {
                sb.AppendLine();
                sb.Append(new string(' ', tabSize.Value * baseIndent.Value));
            }
            sb.Append(']');
            return sb.ToString();
        }

        public void Parse(string json)
        {
            var stack = new Stack<char>();
            var isPair = false;
            Items.Clear();
            // The last ']' is used for terminating
            int start = 1;
            for (var i = 1; i < json.Length; i++)
            {
                var c = json[i];

                if (stack.Count > 0)
                {
                    var b = stack.Peek();
                    if (b == '[' && c == ']' || b == '{' && c == '}' || b == '"' && c == '"')
                    {
                        stack.Pop();
                    }
                    if (c != ']')
                    {
                        continue;
                    }
                }
                else if (stack.Count == 0 && c == ':')
                {
                    isPair = true;
                    continue;
                }

                if (stack.Count == 0 && (c == ',' ||  c == ']'))
                {
                    var ss = json.Substring(start, i - start).Trim();
                    if (ss.Length > 0 && ss[0] == '{' || isPair)
                    {
                        if (ss.Length == 0 || ss[0] != '{')
                        {
                            ss = "{" + ss + "}";
                        }

                        var jsPairs = new JsonPairs();
                        jsPairs.Parse(ss);
                        Items.Add(jsPairs);
                    }
                    else if (ss.Length > 0 && ss[0] == '[')
                    {
                        var jsArray = new JsonArray();
                        jsArray.Parse(ss);
                        Items.Add(jsArray);
                    }
                    else
                    {
                        var jsValue = ss.GetJsonValue();
                        Items.Add(jsValue);
                    }
                    start = i + 1;
                }

                if (c == '[' || c == '{' || c == '"')
                {
                    stack.Push(c);
                }   
            }
        }
    }
}
