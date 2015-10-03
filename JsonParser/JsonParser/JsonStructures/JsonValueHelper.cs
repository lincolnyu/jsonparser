namespace JsonParser.JsonStructures
{
    public static class JsonValueHelper
    {
        public static JsonNode GetJsonValue(this string s)
        {
            if (s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                var jsonString = new JsonValue<string>();
                jsonString.Value = s.Substring(1, s.Length - 2);
                return jsonString;
            }
            else
            {
                int ival;
                if (int.TryParse(s, out ival))
                {
                    var jsonInt = new JsonValue<int>();
                    jsonInt.Value = ival;
                    return jsonInt;
                }
                else
                {
                    // as string
                    var jsonString = new JsonValue<string>();
                    jsonString.Value = s;
                    return jsonString;
                }
                // TODO other types?
            }
        }
    }
}
