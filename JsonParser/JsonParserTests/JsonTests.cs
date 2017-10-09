using JsonParser;
using JsonParser.Formatting;
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
            const string expectedPretty = @"{
    ""id"": 1,
    ""jsonrpc"": ""2.0"",
    ""result"": {
        ""items"": [
            {
                ""episode"": -1,
                ""label"": ""American.Hustle.2013.720p.BluRay.x264.YIFY.mp4"",
                ""plot"": """",
                ""runtime"": 8284,
                ""season"": -1,
                ""showtitle"": """",
                ""thumbnail"": """",
                ""title"": """",
                ""type"": ""unknown""
            }
        ],
        ""limits"": {
            ""end"": 1,
            ""start"": 0,
            ""total"": 1
        }
    }
}";
            var jsNode = input.ParseJson();
            var output = jsNode.ToString();
            Assert.AreEqual(expected, output);
            var pretty = jsNode.ToString(JsonFormat.NormalFormat, null, 0, 4);
            Assert.AreEqual(expectedPretty, pretty);
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string input = "{ \"id\":1,\"jsonrpc\":\"2.0\",\"result\":{ \"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\r\nsecondline\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"},\"param\": 1],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";
            const string expected = "{\"id\":1,\"jsonrpc\":\"2.0\",\"result\":{\"items\":[{\"episode\":-1,\"label\":\"American.Hustle.2013.720p.BluRay.x264.YIFY.mp4\r\nsecondline\",\"plot\":\"\",\"runtime\":8284,\"season\":-1,\"showtitle\":\"\",\"thumbnail\":\"\",\"title\":\"\",\"type\":\"unknown\"},\"param\":1],\"limits\":{\"end\":1,\"start\":0,\"total\":1}}}";
            const string expectedPretty = @"{
    ""id"": 1,
    ""jsonrpc"": ""2.0"",
    ""result"": {
        ""items"": [
            {
                ""episode"": -1,
                ""label"": ""American.Hustle.2013.720p.BluRay.x264.YIFY.mp4
                    secondline"",
                ""plot"": """",
                ""runtime"": 8284,
                ""season"": -1,
                ""showtitle"": """",
                ""thumbnail"": """",
                ""title"": """",
                ""type"": ""unknown""
            },
            ""param"": 1
        ],
        ""limits"": {
            ""end"": 1,
            ""start"": 0,
            ""total"": 1
        }
    }
}";
            var jsNode = input.ParseJson();
            var output = jsNode.ToString();
            Assert.AreEqual(expected, output);
            var pretty = jsNode.ToString(JsonFormat.SuccinctFormat, null, 0, 4);
            Assert.AreEqual(expectedPretty, pretty);
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

        [TestMethod]
        public void TestBoolean()
        {
            const string input = "{\"boolValue\": true, \"boolVal2\":false}";
            const string expected = "{\"boolValue\":true,\"boolVal2\":false}";
            var jsNode = input.ParseJson();
            var jsPairs = jsNode as JsonPairs;
            Assert.IsTrue(jsPairs != null);
            Assert.IsTrue(jsPairs.TryGetValue<bool>("boolValue", out var val1));
            Assert.AreEqual(true, val1);
            Assert.IsTrue(jsPairs.TryGetValue<bool>("boolVal2", out var val2));
            Assert.AreEqual(false, val2);
            Assert.AreEqual(expected, jsNode.ToString());
        }

        [TestMethod]
        public void TestNum()
        {
            const string input = "{\"posValue\": 1234, \"negVal\": -2345.678}";
            const string expected = "{\"posValue\":1234,\"negVal\":-2345.678}";
            var jsNode = input.ParseJson();
            var jsPairs = jsNode as JsonPairs;
            Assert.IsTrue(jsPairs != null);
            Assert.IsTrue(jsPairs.TryGetValue<Numeric>("posValue", out var val1));
            Assert.AreEqual(new Numeric("1234"), val1);
            Assert.IsTrue(jsPairs.TryGetValue<Numeric>("negVal", out var val2));
            Assert.AreEqual(new Numeric("-2345.678"), val2);
            Assert.AreEqual(expected, jsNode.ToString());
        }

        [TestMethod]
        public void TestEmptyArrayItems()
        {
            const string input = "[1234, -2345.678, , 1, ]";
            const string expected = "[1234,-2345.678,1]";
            var jsNode = input.ParseJson();
            var jsArray = jsNode as JsonArray;
            Assert.IsTrue(jsArray != null);
            Assert.AreEqual(expected, jsNode.ToString());
        }

        [TestMethod]
        public void TestEmptyArrayItems2()
        {
            const string input = "[\"posValue\": 1234, \"negVal\": -2345.678, ]";
            const string expected = "[\"posValue\":1234,\"negVal\":-2345.678]";
            var jsNode = input.ParseJson();
            var jsArray = jsNode as JsonArray;
            Assert.IsTrue(jsArray != null);
            Assert.AreEqual(expected, jsNode.ToString());
        }

        [TestMethod]
        public void TestEmptyPairs()
        {
            const string input = "{\"posValue\": 1234, , \"negVal\": -2345.678, }";
            const string expected = "{\"posValue\":1234,\"negVal\":-2345.678}";
            var jsNode = input.ParseJson();
            var jsPairs = jsNode as JsonPairs;
            Assert.IsTrue(jsPairs != null);
            Assert.IsTrue(jsPairs.TryGetValue<Numeric>("posValue", out var val1));
            Assert.AreEqual(new Numeric("1234"), val1);
            Assert.IsTrue(jsPairs.TryGetValue<Numeric>("negVal", out var val2));
            Assert.AreEqual(new Numeric("-2345.678"), val2);
            Assert.AreEqual(expected, jsNode.ToString());
        }
    }
}
