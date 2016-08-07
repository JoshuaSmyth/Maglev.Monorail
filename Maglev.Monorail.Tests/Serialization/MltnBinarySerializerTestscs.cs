using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Maglev.Monorail.Diagnostics;
using Maglev.Monorail.Serialization;
using Maglev.Monorail.Tests.Serialization.JsonTracksMappers;
using Maglev.Monorail.Tests.Serialization.TestObjects;
using NUnit.Framework;

namespace Maglev.Monorail.Tests.Serialization
{
    class MltnBinarySerializerTestscs
    {
        [TestFixture]
        public class MltnTextSerializerTests
        {
            private List<TestObject> m_TestObjects;
            public MltnTextSerializerTests()
            {
                TracedStopwatch.Init();

                SetupData();
            }

            private void SetupData()
            {
                m_TestObjects = new List<TestObject>();

                for (var i = 0; i < 100; i++)
                {
                    var s = new TestObject { Name = "Josh", Balance = 500 + i, Age = 2 };
                    //s.Tags.Add("Tag 1");
                    //s.Tags.Add("Tag 2");
                    //s.Tags.Add("Tag 3");


                    var a = new TestObject { Name = "Josh2", Balance = 250 - i, Age = 3 };
                    //a.Tags.Add("Tag 4");
                    //a.Tags.Add("Tag 5");
                    //a.Tags.Add("Tag 6");

                    m_TestObjects.Add(a);
                    m_TestObjects.Add(s);
                }
            }

            [Test]
            public void TestSerialization()
            {
                MltnBinaryTracks.RegisterMapping(typeof(TestObject), new TestObjectMltnBinaryWriteMapper());
          
                byte[] result;
                using (TracedStopwatch.Create("Prime Test"))
                {
                    result = MltnBinaryTracks.Serialize(m_TestObjects);
                }

                const int iterationCount = 10000;
                using (TracedStopwatch.Create("Mltn Serialize"))
                {
                    for (int i = 0; i < iterationCount; i++)
                    {
                        var serialForm = MltnBinaryTracks.Serialize(m_TestObjects);
                    }
                }
                
                Trace.WriteLine("Mltn Length: " + result.Length);
                Trace.WriteLine(result);
            }
        }
    }
}
