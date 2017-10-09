using JsonParser.Formatting;

namespace JsonParser.JsonStructures
{
    public abstract class JsonNode
    {
        public abstract string ToString(JsonFormat format, JsonNode parentNode, int? baseIndent, int? tabSize);
    }
}
