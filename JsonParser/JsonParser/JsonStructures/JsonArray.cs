using System.Collections.Generic;
using System.Text;
using JsonParser.Helpers;
using JsonParser.Formatting;

namespace JsonParser.JsonStructures
{
    public class JsonArray : JsonNode
    {
        public List<JsonNode> Items { get; private set; } = new List<JsonNode>();

        /// <summary>
        ///  The default string which is the most compact form
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(JsonFormat.CompactFormat, null, null, null);

        public override string ToString(JsonFormat format, JsonNode parentNode, int? baseIndent, int? tabSize)
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
                    sb.Append(jsonPairs.ToString(format, this, (baseIndent + 1) ?? null, tabSize));
                }
                else
                {
                    sb.Append(item.ToString(format, this, (baseIndent + 1) ?? null, tabSize));
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
            System.Diagnostics.Debug.Assert(!char.IsWhiteSpace(json[0]));
            System.Diagnostics.Debug.Assert(!char.IsWhiteSpace(json[json.Length - 1]));

            var stack = new Stack<char>();
            var inQuote = false;

            var isPair = false; // To support naked pairs as items
            var start = 1;

            Items.Clear();

            System.Diagnostics.Debug.Assert(json[0] == '[');
            System.Diagnostics.Debug.Assert(json[json.Length - 1] == ']');

            // NOTE The first '[' is consumed
            //      and the last ']' is used for terminating
            foreach (var t in json.DeEscape(() => inQuote, 1))
            {
                var c = t.Item1;
                var de = t.Item3;

                // flip in-quote status
                if (c == '"' && !de)
                {
                    inQuote = !inQuote;
                    continue;
                }

                // we don't need to do anything when it's in-quote
                if (inQuote)
                {
                    continue;
                }

                var i = t.Item2;

                if (stack.Count == 0 && (c == ',' || c == ']'))
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
                        isPair = false;
                    }
                    else if (ss.Length > 0 && ss[0] == '[')
                    {
                        var jsArray = new JsonArray();
                        jsArray.Parse(ss);
                        Items.Add(jsArray);
                    }
                    else if (!string.IsNullOrEmpty(ss))
                    {
                        var jsValue = ss.GetJsonValue();
                        Items.Add(jsValue);
                    }
                    start = i + 1;
                }

                if (stack.Count > 0)
                {
                    var b = stack.Peek();
                    if (b == '[' && c == ']' || b == '{' && c == '}')
                    {
                        stack.Pop();
                        continue;
                    }
                }
                else if (c == ':')
                {
                    isPair = true; // Aggressively identify a naked pair
                    continue;
                }

                if (c == '[' || c == '{')
                {
                    stack.Push(c);
                }   
            }
        }
    }
}
