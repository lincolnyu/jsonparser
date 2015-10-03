using System;

namespace JsonParser.JsonStructures
{
    public class JsonValue<T> : JsonNode
    {
        public T Value { get; set; }

        public override string ToString()
        {
            if (typeof(T) == typeof(int))
            {
                return Value.ToString();
            }
            else if (typeof(T) == typeof(string))
            {
                return string.Format("\"{0}\"", Value);
            }
            throw new NotSupportedException();
        }
    }
}
