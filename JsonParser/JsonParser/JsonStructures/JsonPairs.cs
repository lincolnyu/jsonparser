using System.Collections.Generic;
using System.Text;

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

        public string ToNakedStringIfPossible()
        {
            return ToString(KeyValues.Count != 1);
        }

        public override string ToString()
        {
            return ToString(true);
        }

        private string ToString(bool enclosed)
        {
            var sb = new StringBuilder();
            if (enclosed)
            {
                sb.Append('{');
            }
            var once = false;
            foreach (var kvp in KeyValues)
            {
                if (once)
                {
                    sb.Append(',');
                }
                else
                {
                    once = true;
                }
                sb.Append('"');
                sb.Append(kvp.Key);
                sb.Append("\":");
                sb.Append(kvp.Value.ToString());
            }
            if (enclosed)
            {
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
