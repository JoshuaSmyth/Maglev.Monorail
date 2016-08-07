using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Serialization;
using Maglev.Monorail.Tests.Serialization.TestObjects;

namespace Maglev.Monorail.Tests.Serialization.JsonTracksMappers
{
    public class TestObjectJsonWriteMapper : IJsonWriter
    {
        public void WriteJson(object input, JsonBuilder jb)
        {
            var testObject = input as TestObject;

            jb.WritePropertyC("Name", testObject.Name);
            jb.WritePropertyC("Age", testObject.Age);
            jb.WriteProperty("Balance", testObject.Balance);
        }
    }
}
