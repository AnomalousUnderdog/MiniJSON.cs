
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class MiniJSONTests
    {
        [Test]
        public void DeserializeWorks_ForSingleElementArray()
        {
            string testString =
                @"[
    {
        ""a"": 1
    }
]";
            var output = MiniJSON.Json.Deserialize(testString) as List<object>;
            Assert.NotNull(output);
            Assert.True(output.Count == 1);

            var firstElement = output[0] as Dictionary<string, object>;
            Assert.NotNull(firstElement);
            Assert.True(firstElement.ContainsKey("a"));

            var firstElementValue = firstElement["a"] is long ? (long)firstElement["a"] : 0;
            Assert.True(firstElementValue == 1);
        }

        [Test]
        public void DeserializeWorks_ForNameWithQuotes()
        {
            string testString =
                @"{
    ""a"": 1
}";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("a"));

            var firstElementValue = output["a"] is long ? (long)output["a"] : 0;
            Assert.True(firstElementValue == 1);
        }

        [Test]
        public void DeserializeWorks_ForStringValue()
        {
            string testString = "{ \"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog\" }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("string"));

            var gotValue = output["string"] as string;
            Assert.True(gotValue == "The quick brown fox \"jumps\" over the lazy dog", string.Format("gotValue: {0}", gotValue));
        }

        [Test]
        public void DeserializeWorks_ForUnicodeValue()
        {
            string testString = "{ \"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\" }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("unicode"));

            var gotValue = output["unicode"] as string;
            Assert.True(gotValue == "\u3041 Men\u00fa sesi\u00f3n", string.Format("gotValue: {0}", gotValue));
        }

        [Test]
        public void DeserializeWorks_ForIntValue()
        {
            string testString = "{ \"int\": 65536 }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("int"));

            var gotValue = output["int"] is long ? (long)output["int"] : 0;
            Assert.True(gotValue == 65536);
        }

        [Test]
        public void DeserializeWorks_ForLargeUnsignedInt64Value()
        {
            // commented by vzack (https://gist.github.com/darktable/1411710#gistcomment-1252167)
            string testString = "{\"sessionid\":12156480978394720353}";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("sessionid"));

            var gotValue = output["sessionid"] is ulong ? (ulong)output["sessionid"] : 0;
            Assert.True(gotValue == 12156480978394720353);
        }

        [Test]
        public void DeserializeWorks_ForDoubleFloatValue()
        {
            string testString = "{ \"float\": 3.1415926 }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("float"));

            var gotValue = output["float"] is double ? (double)output["float"] : 0.0;
            Assert.AreEqual(3.1415926, gotValue, 0.0000000001);
        }

        [Test]
        public void DeserializeWorks_ForFloatingPointNotationValue()
        {
            string testString = "{ \"float\": 1e-005 }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("float"));

            var gotValue = output["float"] is double ? (double)output["float"] : 0.0;
            Assert.True(gotValue > 0);
            Assert.AreEqual(1e-005, gotValue, 0.0000000001);
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForZeroFloatValue()
        {
            Dictionary<string, double> doubleTest = new Dictionary<string, double>();
            doubleTest.Add("number", 0.0);
            string gotSerialized = MiniJSON.Json.Serialize(doubleTest);
            Assert.True(gotSerialized == "{\"number\":0.0}", string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("number"));

            var gotValue = output["number"] is double ? (double)output["number"] : 0.0;
            Assert.AreEqual(0.0, gotValue, 0.001);
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForWholeNumberSingleFloatValue()
        {
            // commented by nikibobi (https://gist.github.com/darktable/1411710#gistcomment-806908)
            // and GabrielHare (https://gist.github.com/darktable/1411710#gistcomment-1586999)
            Dictionary<string, float> doubleTest = new Dictionary<string, float>();
            doubleTest.Add("number", 1.0f);
            string gotSerialized = MiniJSON.Json.Serialize(doubleTest);
            // the serialized text needs to have a decimal value (even though it's 0),
            // so that the parser understands that it's a double value
            Assert.True(gotSerialized == "{\"number\":1.0}", string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("number"));

            // note: MiniJSON doesn't hold single float values, so we expect the deserialized value to be double
            // if value was serialized as "1" instead of "1.0", this would have generated an error
            var gotValue = output["number"] is double ? (double)output["number"] : 0.0;
            Assert.AreEqual(1.0, gotValue, 0.001);
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForWholeNumberDoubleFloatValue()
        {
            // commented by nikibobi (https://gist.github.com/darktable/1411710#gistcomment-806908)
            // and GabrielHare (https://gist.github.com/darktable/1411710#gistcomment-1586999)
            Dictionary<string, double> doubleTest = new Dictionary<string, double>();
            doubleTest.Add("number", 1.0);
            string gotSerialized = MiniJSON.Json.Serialize(doubleTest);
            // the serialized text needs to have a decimal value (even though it's 0),
            // so that the parser understands that it's a double value
            Assert.True(gotSerialized == "{\"number\":1.0}", string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("number"));

            // if value was serialized as "1" instead of "1.0", this would have generated an error
            var gotValue = output["number"] is double ? (double)output["number"] : 0.0;
            Assert.AreEqual(1.0, gotValue, 0.001);
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForVerySmallFloatValue()
        {
            // commented by BestStream (https://gist.github.com/darktable/1411710#gistcomment-2929117)
            Dictionary<string, double> doubleTest = new Dictionary<string, double>();
            doubleTest.Add("d", 0.000000001);
            string gotSerialized = MiniJSON.Json.Serialize(doubleTest);
            Assert.True(gotSerialized == "{\"d\":1E-09}", string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("d"));

            var gotValue = output["d"] is double ? (double)output["d"] : 0.0;
            Assert.True(gotValue > 0);
            Assert.AreEqual(1E-09, gotValue, 0.0000000001);
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForZeroIntValue()
        {
            Dictionary<string, int> intTest = new Dictionary<string, int>();
            intTest.Add("number", 0);
            string gotSerialized = MiniJSON.Json.Serialize(intTest);
            Assert.True(gotSerialized == "{\"number\":0}", string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("number"));

            var gotValue = output["number"] is int ? (int)output["number"] : 0;
            Assert.True(gotValue == 0);
        }

        [System.Serializable]
        public class SerializedClassTest
        {
            public long timestamp;
            public string signature;
            public string id;
        }

        [Test]
        public void SerializeThenDeserializeWorks_ForArrayOfClass()
        {
            // commented by sotirosn (https://gist.github.com/darktable/1411710#gistcomment-1997521)
            string gotSerialized = MiniJSON.Json.Serialize(new object[] {new SerializedClassTest { timestamp = 143321321321, signature = "sha256 hash", id="A1234" }, "Hello World!"});
            Assert.True(gotSerialized == "[{\"timestamp\":143321321321,\"signature\":\"sha256 hash\",\"id\":\"A1234\"},\"Hello World!\"]",
                string.Format("Got Serialized Text: {0}", gotSerialized));

            var output = MiniJSON.Json.Deserialize(gotSerialized) as List<object>;
            Assert.NotNull(output);
            Assert.True(output.Count == 2);

            var element2 = output[1] as string;
            Assert.True(element2 == "Hello World!", string.Format("element2: {0}", element2));

            var element1 = output[0] as Dictionary<string, object>;
            Assert.NotNull(element1);

            var gotTimestamp = element1["timestamp"] is long ? (long)element1["timestamp"] : 0;
            Assert.True(gotTimestamp == 143321321321);

            var gotSignature = element1["signature"] as string;
            Assert.True(gotSignature == "sha256 hash", string.Format("gotSignature: {0}", gotSignature));

            var gotId = element1["id"] as string;
            Assert.True(gotId == "A1234", string.Format("gotId: {0}", gotId));
        }

        [Test]
        public void DeserializeWorks_ForBoolValue()
        {
            string testString = "{ \"bool\": true }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("bool"));

            var gotValue = output["bool"] as bool? ?? false;
            Assert.True(gotValue);
        }

        [Test]
        public void DeserializeWorks_ForNullValue()
        {
            string testString = "{ \"null\": null }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.NotNull(output);
            Assert.True(output.ContainsKey("null"));

            var gotValue = output["null"];
            Assert.True(gotValue == null);
        }

        [Test]
        public void DeserializeAborts_ForArrayWithColonInside()
        {
            // commented by fufie (https://gist.github.com/darktable/1411710#gistcomment-1717287)
            string testString = "{\"grid\": [ \"A0\": { \"structure\": \"simple-tap\" } ] }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.Null(output);
        }

        [Test]
        public void DeserializeAborts_WhenTextIsInvalidJsonString()
        {
            // commented by prestonmediaspike (https://gist.github.com/darktable/1411710#gistcomment-1279571)
            var output = MiniJSON.Json.Deserialize("{\"r\":[{\"a:[\"http:");
            Assert.Null(output);
        }

        [Test]
        public void DeserializeAborts_WhenNameHasNoQuotes()
        {
            // Names need to be in quotes. See https://stackoverflow.com/q/949449/1377948
            string testString = "{ a: 1 }";
            var output = MiniJSON.Json.Deserialize(testString) as Dictionary<string, object>;
            Assert.Null(output);
        }

        [Test]
        public void DeserializeAborts_WhenTextIsNull()
        {
            var output = MiniJSON.Json.Deserialize(null);
            Assert.Null(output);
        }

        [Test]
        public void DeserializeAborts_WhenTextIsEmptyString()
        {
            var output = MiniJSON.Json.Deserialize("");
            Assert.Null(output);
        }
    }
}
