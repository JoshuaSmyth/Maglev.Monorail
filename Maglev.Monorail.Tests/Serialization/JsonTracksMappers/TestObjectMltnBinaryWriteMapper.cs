using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Serialization;
using Maglev.Monorail.Tests.Serialization.TestObjects;

namespace Maglev.Monorail.Tests.Serialization.JsonTracksMappers
{
    class TestObjectMltnBinaryWriteMapper : IMltnBinaryWriter
    {
        public void WriteBody(object input, MltnBinaryBuilder builder)
        {
            var obj = input as TestObject;
            builder.WriteProperty(obj.Name);
            builder.WriteProperty(obj.Age);
            builder.WriteProperty(obj.Balance);
        }

        public void WriteHeader(MltnBinaryBuilder builder)
        {
            builder.WriteHeader("Name", DefineType.String);
            builder.WriteHeader("Age", DefineType.Integer);
            builder.WriteHeader("Balance", DefineType.Integer);
        }
    }
}
