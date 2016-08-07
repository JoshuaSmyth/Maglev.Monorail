using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Serialization;
using Maglev.Monorail.Tests.Serialization.TestObjects;

namespace Maglev.Monorail.Tests.Serialization.JsonTracksMappers
{
    public class TestObjectMltnWriteMapper : IMltnWriter
    {
        public void WriteHeader(MltnTextBuilder textBuilder)
        {
            textBuilder.WriteHeader("Name", "string");  // TODO Change to enum
            textBuilder.WriteHeader("Age", "integer");
            textBuilder.WriteHeader("Balance", "integer");
            textBuilder.WriteHeader("Tags", "Array<String>");
        }

        public void WriteBody(object input, MltnTextBuilder textBuilder)
        {
            var testObject = input as TestObjectWithTags;

            textBuilder.WriteProperty(testObject.Name);
            textBuilder.WriteProperty(testObject.Age);
            textBuilder.WriteProperty(testObject.Balance);
            textBuilder.WriteStringArray(testObject.Tags);
        }

        // Read Header

        // Read Body
    }
}
