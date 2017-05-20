using JsonParser;
using JsonParser.JsonStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonParserTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string input = "{ \"id\":1,\"jsonrpc\":\"2.0\",\"result\":{ \"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"}],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";
            const string expected = "{\"id\":1,\"jsonrpc\":\"2.0\",\"result\":{\"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"}],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";

            var jsNode = input.ParseJson();
            var output = jsNode.ToString();
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string input = "{ \"id\":1,\"jsonrpc\":\"2.0\",\"result\":{ \"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"},\"param\": 1],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";
            const string expected = "{\"id\":1,\"jsonrpc\":\"2.0\",\"result\":{\"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"},\"param\":1],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";

            var jsNode = input.ParseJson();
            var output = jsNode.ToString();
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TestMethod3()
        {
            const string input = "{\"titleWith\\\"QMs\\\"\":\"I have \\\"quotation marks\\\"\"}";
            var jsNode = input.ParseJson();
            var jsPairs = jsNode as JsonPairs;
            Assert.IsTrue(jsPairs != null);
            Assert.IsTrue(jsPairs.TryGetValue<string>("titleWith\"QMs\"", out var val));
            Assert.AreEqual("I have \"quotation marks\"", val);
        }
    }
}
