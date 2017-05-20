using System;

namespace JsonParser.JsonStructures
{
    public struct Numeric : IEquatable<Numeric>
    {
        public Numeric(string val)
        {
            Value = val.Trim();
        }

        public string Value { get; }

        public bool Equals(Numeric other) => Value == other.Value;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Numeric))
            {
                return false;
            }
            var other = (Numeric)obj;
            return Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();
        
        public override string ToString() => Value;
       
        public static bool IsDecimalNumeric(string test)
        {
            var first = true;
            var dotted = false;
            foreach (var c in test)
            {
                if (c == '-')
                {
                    if (!first)
                    {
                        return false;
                    }
                    first = false;
                }
                else if (c == '.')
                {
                    if (dotted)
                    {
                        return false;
                    }
                    dotted = true;
                    first = true;
                }
                else if (char.IsDigit(c))
                {
                    first = true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
