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
    [TestFixture]
    public class MltnTextSerializerTests
    {
        private List<TestObjectWithTags> m_TestObjects; 
        public MltnTextSerializerTests()
        {
            TracedStopwatch.Init();

            SetupData();
        }

        private void SetupData()
        {
            m_TestObjects = new List<TestObjectWithTags>();

            for (var i = 0; i < 100; i++)
            {
                var s = new TestObjectWithTags { Name = "Josh", Balance = 500 + i, Age = 2 };
                s.Tags.Add("Tag 1");
                s.Tags.Add("Tag 2");
                s.Tags.Add("Tag 3");


                var a = new TestObjectWithTags { Name = "Josh2", Balance = 250 - i, Age = 3 };
                a.Tags.Add("Tag 4");
                a.Tags.Add("Tag 5");
                a.Tags.Add("Tag 6");

                m_TestObjects.Add(a);
                m_TestObjects.Add(s);
            }
        }

        [SetUp]
        public void Setup()
        {
            JsonTracks.ClearMappings();
            MltnTextTracks.ClearMappings();
        }

        [Test]
        public void TestSerialization()
        {
            MltnTextTracks.RegisterMapping(typeof(TestObjectWithTags), new TestObjectMltnWriteMapper());
       
            String result;
            using (TracedStopwatch.Create("Prime Test"))
            {
                result = MltnTextTracks.Serialize(m_TestObjects);
            }

            String resultMltnCompact;
            using (TracedStopwatch.Create("Prime Mltn Compact"))
            {
                resultMltnCompact = MltnTextTracks.Serialize(m_TestObjects, PrintMode.Compact);
            }

            const int iterationCount = 5000;
            using (TracedStopwatch.Create("Mltn Serialize"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = MltnTextTracks.Serialize(m_TestObjects);
                }
            }

            using (TracedStopwatch.Create("Mltn Compact Serialize"))
            {
                for (int i = 0; i < iterationCount; i++)
                {
                    var serialForm = MltnTextTracks.Serialize(m_TestObjects, PrintMode.Compact);
                }
            }

            Trace.WriteLine("Mltn Length: " + result.Length);
            Trace.WriteLine("Mltn Compact: " + resultMltnCompact.Length);

            Trace.WriteLine("Mltn Full:");
            Trace.WriteLine(result);
        }
    }
}
