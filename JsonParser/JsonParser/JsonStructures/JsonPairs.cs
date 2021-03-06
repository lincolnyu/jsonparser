﻿using System.Collections.Generic;
using System.Text;
using JsonParser.Helpers;
using JsonParser.Formatting;
using static JsonParser.Formatting.JsonFormat;

namespace JsonParser.JsonStructures
{
    public class JsonPairs : JsonNode
    {
        public Dictionary<string, JsonNode> KeyValues { get; private set; } = new Dictionary<string, JsonNode>();

        public static bool TryGetNode<TNode>(KeyValuePair<string, JsonNode> pair, out TNode node) where TNode : JsonNode
        {
            node = pair.Value as TNode;
            return node != null;
        }

        public static bool TryGetValue<TValue>(KeyValuePair<string, JsonNode> pair, out TValue val)
        {
            if (pair.Value is JsonValue<TValue> valNode)
            {
                val = valNode.Value;
                return true;
            }
            val = default(TValue);
            return false;
        }

        public bool TryGetNode<TNode>(string key, out TNode node) where TNode : JsonNode
        {
            if (KeyValues.TryGetValue(key, out var n))
            {
                node = n as TNode;
                return node != null;
            }
            node = null;
            return false;
        }

        public bool TryGetValue<TValue>(string key, out TValue val)
        {
            if (TryGetNode<JsonValue<TValue>>(key, out var valNode))
            {
                val = valNode.Value;
                return true;
            }
            val = default(TValue);
            return false;
        }

        public override string ToString() => ToString(CompactFormat, null, null, null);

        public override string ToString(JsonFormat jsonFormat, JsonNode parentNode, int? baseIndent, int? tabSize)
        {
            var enclose = jsonFormat.PairEnclosingType != PairEnclosingTypes.NoEncloseIfPossible
                || !(parentNode is JsonArray) || KeyValues.Count > 1;
            var newLineToBrace = baseIndent != null && !jsonFormat.Compact &&
                (jsonFormat.PairEnclosingType == PairEnclosingTypes.EncloseRigid
                || KeyValues.Count > 1);
            if (KeyValues.Count == 0)
            {
                return enclose ? "{}" : string.Empty;
            }
            var sb = new StringBuilder();
            if (enclose)
            {
                sb.Append('{');
                if (newLineToBrace)
                {
                    sb.AppendLine();
                }
                else if (!jsonFormat.Compact)
                {
                    sb.Append(" ");
                }
            }
            foreach (var kvp in KeyValues)
            {
                if (baseIndent != null && newLineToBrace)
                {
                    sb.Append(new string(' ', tabSize.Value * (baseIndent.Value + 1)));
                }
                sb.Append('"');
                sb.Append(kvp.Key);
                sb.Append("\":");
                if (!jsonFormat.Compact)
                {
                    sb.Append(" ");
                }
                sb.Append(kvp.Value.ToString(jsonFormat, this, (enclose ? baseIndent + 1 : baseIndent) ?? null, tabSize));
                sb.Append(',');
                if (baseIndent != null)
                {
                    sb.AppendLine();
                }
            }
            var comma = sb.ToString().LastIndexOf(",");
            sb.Remove(comma, sb.Length - comma); // remove the last comma
            if (enclose)
            {
                if (newLineToBrace)
                {
                    sb.AppendLine();
                    sb.Append(new string(' ', tabSize.Value * baseIndent.Value));
                }
                else if (!jsonFormat.Compact)
                {
                    sb.Append(" ");
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
            System.Diagnostics.Debug.Assert(!char.IsWhiteSpace(json[0]));
            System.Diagnostics.Debug.Assert(!char.IsWhiteSpace(json[json.Length - 1]));

            var stack = new Stack<char>();
            var inQuote = false;

            string key = null;
            JsonNode jsNode = null;
            var keyStart = 1;
            var valueStart = 0;

            KeyValues.Clear();

            System.Diagnostics.Debug.Assert(json[0] == '{');
            System.Diagnostics.Debug.Assert(json[json.Length-1] == '}');

            // NOTE The first '{' is consumed
            //      and the last '}' is used for key-value pair terminating similar to ','
            foreach (var t in json.DeEscape(()=>inQuote, 1))
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

                if (key == null && c == ':')
                {
                    // This should be ensured by the fact that stack is performed only at the value part (see below)
                    System.Diagnostics.Debug.Assert(stack.Count == 0);
                    key = json.Substring(keyStart, i - keyStart).GetJsonKey();
                    jsNode = null;
                    valueStart = i + 1;
                    continue;
                }

                // Try to collect a key
                if (stack.Count == 0 && (c == ',' || c == '}'))
                {
                    // Invalid or empty pairs are ignored
                    if (key != null)
                    {
                        jsNode = jsNode ?? json.Substring(valueStart, i - valueStart).GetJsonValue();
                        KeyValues[key] = jsNode;
                    }

                    jsNode = null;
                    key = null;
                    keyStart = i + 1;
                    continue;
                }

                if (stack.Count > 0)
                {
                    var b = stack.Peek();
                    if ((b == '[' && c == ']' || b == '{' && c == '}'))
                    {
                        stack.Pop();
                        if (stack.Count == 0)
                        {
                            var ss = json.Substring(valueStart, i + 1 - valueStart).Trim();
                            if (b == '{')
                            {
                                var jsPairs = new JsonPairs();
                                jsPairs.Parse(ss);
                                jsNode = jsPairs;
                            }
                            else
                            {
                                System.Diagnostics.Debug.Assert(b == '[');
                                var jsArray = new JsonArray();
                                jsArray.Parse(ss);
                                jsNode = jsArray;
                            }
                        }
                        continue;
                    }
                }

                // The nullity check is to make sure this stacking is only performed in the value part
                if (jsNode == null && (c == '[' || c == '{')) 
                {
                    stack.Push(c);
                }
            }
        }
    }
}
