using JsonParser.JsonStructures;

namespace JsonParser.Helpers
{
    public static class JsonPairsHelper
    {
        public static JsonNode GetNodeOrNull(this JsonPairs jsPairs, string key)
        {
            if (jsPairs.KeyValues.TryGetValue(key, out JsonNode node))
            {
                return node;
            }
            return null;
        }

        public static T GetNodeOrNull<T>(this JsonPairs jsPairs, string key) where T : JsonNode
        {
            var node = jsPairs.GetNodeOrNull(key);
            return node as T;
        }

        public static T GetValueOrDefault<T>(this JsonPairs jsPairs, string key)
        {
            var jsNode = jsPairs.GetNodeOrNull<JsonValue<T>>(key);
            return jsNode != null ? jsNode.Value : default(T);
        }
    }
}
