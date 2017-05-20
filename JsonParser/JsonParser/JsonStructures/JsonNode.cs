namespace JsonParser.JsonStructures
{
    public abstract class JsonNode
    {
        public abstract string ToString(int? baseIndent, int? tabSize);
    }
}
