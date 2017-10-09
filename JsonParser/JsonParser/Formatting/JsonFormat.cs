namespace JsonParser.Formatting
{
    public class JsonFormat
    {
        public enum PairEnclosingTypes
        {
            NoEncloseIfPossible,
            EncloseRigid,
            EncloseSameLineIfSingle
        }

        public PairEnclosingTypes PairEnclosingType { get; set; }

        public bool Compact { get; set; }

        public static readonly JsonFormat CompactFormat = new JsonFormat
        {
            PairEnclosingType = PairEnclosingTypes.NoEncloseIfPossible,
            Compact = true
        };

        public static readonly JsonFormat NormalFormat = new JsonFormat
        {
            PairEnclosingType = PairEnclosingTypes.EncloseSameLineIfSingle,
            Compact = false
        };

        public static readonly JsonFormat SuccinctFormat = new JsonFormat
        {
            PairEnclosingType = PairEnclosingTypes.NoEncloseIfPossible,
            Compact = false
        };
    }
}
