using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Maglev.Monorail.Conversion;
using Maglev.Monorail.Diagnostics;
using Maglev.Monorail.Serialization;
using Maglev.Monorail.Tests.Serialization.JsonTracksMappers;
using Maglev.Monorail.Tests.Serialization.TestObjects;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Maglev.Monorail.Tests.Serialization
{
    [TestFixture]
    class JsonSerializationTests
    {
        private class CustomSerialise
        {
            public String Serialise(List<TestObject> list)
            {
                var sb = new StringBuilder(128);

                if (list.Count > 0)
                {
                    sb.Append("[{");
                    list[0].Serialise(sb);
                    for (var i = 1; i < list.Count; i++)
                    {
                        sb.Append("},{");
                        list[i].Serialise(sb);
                    }
                    sb.Append("}]");

                    return sb.ToString();
                }

                sb.Append("[]");
                return sb.ToString();
            }

            public object Serialise2(List<TestObject> list)
            {
                var sb = new StringBuilder(128);

                if (list.Count > 0)
                {
                    sb.Append("[{");
                    list[0].Serialise2(sb);
                    for (var i = 1; i < list.Count; i++)
                    {
                        sb.Append("},{");
                        list[i].Serialise2(sb);
                    }
                    sb.Append("}]");

                    return sb.ToString();
                }

                sb.Append("[]");
                return sb.ToString();
            }
        }

        [SetUp]
        public void Setup()
        {
            JsonTracks.ClearMappings();
        }


        [Test]
        public void TestSerialization()
        {
            // Notes: Custom serialisation is 4.8x faster than Newtonsoft.Json

            var customSerialise = new CustomSerialise();
            var testList = new List<TestObject>();

            for (var i = 0; i < 100; i++)
            {
                var s = new TestObject { Name = "Josh", Balance = 500 + i, Age = 2 };
                var a = new TestObject { Name = "Josh2", Balance = 250 - i, Age = 3 };

                testList.Add(a);
                testList.Add(s);
            }


            // Register types
            JsonTracks.RegisterMapping(typeof(TestObject), new TestObjectJsonWriteMapper());

            // Prewarm
            var resultTracks = JsonTracks.Serialize(testList);
            var resultCustom = customSerialise.Serialise(testList);
            var resultNewtonSoft = JsonConvert.SerializeObject(testList);
            var resultServiceStack = ServiceStack.Text.JsonSerializer.SerializeToString(testList);
            var resultNetJson = NetJSON.NetJSON.Serialize(testList);

            var iterationCount = 100;


            using (TracedStopwatch.Create("Hand Rolled"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = customSerialise.Serialise2(testList);
                }
            }


            using (TracedStopwatch.Create("Hand Rolled with FastInt2Str"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = customSerialise.Serialise(testList);
                }
            }

            using (TracedStopwatch.Create("JsonTracks"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = customSerialise.Serialise(testList);
                }
            }


            using (TracedStopwatch.Create("NetJson"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = NetJSON.NetJSON.Serialize(testList);
                }
            }
            


            using (TracedStopwatch.Create("ServiceStack.Text Serialise"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = ServiceStack.Text.JsonSerializer.SerializeToString(testList);
                }
            }

            using (TracedStopwatch.Create("Newtonsoft.Json Serialise"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = JsonConvert.SerializeObject(testList);
                }
            }

            Trace.WriteLine(resultNewtonSoft.Length);
            Trace.WriteLine(resultCustom.Length);
            Trace.WriteLine(resultNetJson.Length);
            //Trace.WriteLine(resultTracks.Length);

            Assert.AreEqual(resultNewtonSoft, resultCustom);
            Assert.AreEqual(resultNewtonSoft, resultServiceStack);
            Assert.AreEqual(resultNewtonSoft, resultNetJson);


            var formattedJson = FormatJson(resultNewtonSoft);


            Assert.IsTrue(formattedJson == resultTracks);
        }

        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
