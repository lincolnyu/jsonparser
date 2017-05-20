using JsonParser.JsonStructures;

namespace JsonParser
{
    public static class Parser
    {
        public static JsonNode ParseJson(this string json)
        {
            json = json.Trim();
            if (json.Length > 1 )
            {
                if (json[0] == '{')
                {
                    var jsonPairs = new JsonPairs();
                    jsonPairs.Parse(json);
                    return jsonPairs;
                }
                else if (json[0] == '[')
                {
                    var jsonArray = new JsonArray();
                    jsonArray.Parse(json);
                    return jsonArray;
                }
            }
            return json.GetJsonValue();
        }

        public static bool IsUnbrackedSinglePair(this string json)
        {
            var qstate = 0;
            foreach (var c in json)
            {
                if (c == '"')
                {
                    qstate++;
                    if (qstate > 2)
                    {
                        return false;
                    }
                }
                switch (qstate)
                {
                    case 0:
                        if (!char.IsWhiteSpace(c) && c != '"')
                        {
                            return false;
                        }
                        break;
                    case 2:
                        if (c == ':')
                        {
                            return true;
                        }
                        else if (!char.IsWhiteSpace(c))
                        {
                            return false;
                        }
                        break;
                }
            }
            return false;
        }
    }
}
