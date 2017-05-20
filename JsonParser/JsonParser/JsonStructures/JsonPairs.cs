using System.Collections.Generic;
using System.Text;
using JsonParser.Helpers;

namespace JsonParser.JsonStructures
{
    public class JsonPairs : JsonNode
    {
        public Dictionary<string, JsonNode> KeyValues { get; private set; } = new Dictionary<string, JsonNode>();

        public bool TryGetNode<TNode>(string key, out TNode node) where TNode : JsonNode
        {
            if (!KeyValues.TryGetValue(key, out var n))
            {
                node = null;
                return false;
            }
            node = n as TNode;
            return node != null;
        }

        public bool TryGetValue<TValue>(string key, out TValue val)
        {
            if (!TryGetNode<JsonValue<TValue>>(key, out var valNode))
            {
                val = default(TValue);
                return false;
            }
            val = valNode.Value;
            return true;
        }

        public string ToNakedStringIfPossible(int? baseIndent = null, int? tabSize = null) 
            => ToString(KeyValues.Count != 1, baseIndent, tabSize);

        public override string ToString() => ToString(true);

        public override string ToString(int? baseIndent, int? tabSize) => ToString(true, baseIndent, tabSize);

        public string ToString(bool enclosed, int? baseIndent = null, int? tabSize = null)
        {
            if (KeyValues.Count == 0)
            {
                return enclosed ? "{}" : string.Empty;
            }
            var sb = new StringBuilder();
            if (enclosed)
            {
                sb.Append('{');
                if (baseIndent != null)
                {
                    sb.AppendLine();
                }
            }
            var first = true;
            foreach (var kvp in KeyValues)
            {
                if (baseIndent != null && (enclosed || !first))
                {
                    sb.Append(new string(' ', tabSize.Value * (baseIndent.Value + 1)));
                }
                sb.Append('"');
                sb.Append(kvp.Key);
                sb.Append("\":");
                sb.Append(kvp.Value.ToString((enclosed ? baseIndent + 1 : baseIndent) ?? null, tabSize));
                sb.Append(',');
                if (baseIndent != null)
                {
                    sb.AppendLine();
                }
                first = false;
            }
            var comma = sb.ToString().LastIndexOf(",");
            sb.Remove(comma, sb.Length - comma); // remove the last comma
            if (enclosed)
            {
                if (baseIndent != null)
                {
                    sb.AppendLine();
                    sb.Append(new string(' ', tabSize.Value * baseIndent.Value));
                }
                sb.Append('}');
            }
            return sb.ToString();
        }

        /// <summary>
        ///  Parse a json pair
        /// </summary>
        /// <param name="json">The json to parse</param>
        /// <remarks>
        ///  It now supports escape chars in quotes
        /// </remarks>
        public void Parse(string json)
        {
            json = json.Trim();

            var stack = new Stack<char>();

            JsonNode jsNode = null;

            var inQuote = false;

            KeyValues.Clear();

            string key = null;
            var keyStart = 1;
            var valueStart = 0;

            // NOTE the last '}' is used for key-value pair terminating as ','
            foreach (var t in json.DeEscape(()=>inQuote))
            {
                var c = t.Item1;
                var i = t.Item2;
                var charInQuote = false;
                if (c == '"')
                {
                    inQuote = !inQuote;
                }
                else
                {
                    charInQuote = inQuote;
                }

                if (key == null)
                {
                    if (c == ':' && !charInQuote)
                    {
                        key = json.Substring(keyStart, i - keyStart).GetJsonKey();
                        jsNode = null;
                        valueStart = i + 1;
                        stack.Clear();
                    }
                    continue;
                }

                if (stack.Count > 0)
                {
                    var b = stack.Peek();
                    var popped = (b == '[' && c == ']' || b == '{' && c == '}' || b == '"' && c == '"') && !charInQuote;
                    if (popped)
                    {
                        stack.Pop();
                    }
                    if (stack.Count == 0)
                    {
                        var ss = json.Substring(valueStart, i + 1 - valueStart);
                        if (b == '{')
                        {
                            var jsPairs = new JsonPairs();
                            jsPairs.Parse(ss);
                            jsNode = jsPairs;
                        }
                        else if (b == '[')
                        {
                            var jsArray = new JsonArray();
                            jsArray.Parse(ss);
                            jsNode = jsArray;
                        }
                    }
                    if (popped)
                    {
                        continue;
                    }
                }
                
                if (jsNode == null)
                {
                    if ((c == '[' || c == '{' || c == '"') && !charInQuote)
                    {
                        stack.Push(c);
                    }
                }

                if (stack.Count == 0 && (c == ',' || c == '}') && !charInQuote)
                {
                    jsNode = jsNode ?? json.Substring(valueStart, i - valueStart).GetJsonValue();

                    KeyValues[key] = jsNode;
                    jsNode = null;
                    key = null;
                    keyStart = i + 1;
                }
            }
        }
    }
}
