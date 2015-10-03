using System;
using System.Collections.Generic;
using System.Text;

namespace JsonParser.JsonStructures
{
    public class JsonPairs : JsonNode
    {
        public Dictionary<string, JsonNode> KeyValues { get; private set; } = new Dictionary<string, JsonNode>();

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
        ///  
        /// </summary>
        /// <param name="json"></param>
        /// <remarks>
        ///  NOTE we assume the json strings don't contain double quotes
        /// </remarks>
        public void Parse(string json)
        {
            json = json.Trim();

            int valueStart = 0;
            var stack = new Stack<char>();

            string key = null;
            JsonNode jsNode = null;

            KeyValues.Clear();
            var keyStart = 1;
            // NOTE the last '}' is used for key-value pair terminating as ','
            for (var i = 1; i < json.Length; i++)
            {
                var c = json[i];
                if (key == null)
                {
                    if (c == ':')
                    {
                        key = json.Substring(keyStart, i - keyStart);
                        key = key.Trim().Trim('"');
                        jsNode = null;
                        valueStart = i + 1;
                        stack.Clear();
                    }
                    continue;
                }

                if (stack.Count > 0)
                {
                    var b = stack.Peek();
                    var popped = b == '[' && c == ']' || b == '{' && c == '}' || b == '"' && c == '"';
                    if (popped)
                    {
                        stack.Pop();
                    }
                    if (stack.Count == 0)
                    {
                        var ss = json.Substring(valueStart, i+1-valueStart);
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
                    if (c == '[' || c == '{' || c == '"')
                    {
                        stack.Push(c);
                    }
                }

                if (stack.Count == 0 && (c == ',' || c == '}'))
                {
                    if (jsNode == null)
                    {
                        var ss = json.Substring(valueStart, i - valueStart).Trim();
                        jsNode = ss.GetJsonValue();
                    }

                    KeyValues[key] = jsNode;
                    jsNode = null;
                    key = null;
                    keyStart = i + 1;
                }
            }
        }
    }
}
