using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public static class MltnBinaryTracks
    {
        private static readonly Dictionary<Type, Action<object, MltnBinaryBuilder>> BodyTypeMapping = new Dictionary<Type, Action<object, MltnBinaryBuilder>>();
        private static readonly Dictionary<Type, Action<MltnBinaryBuilder>> HeaderTypeMapping = new Dictionary<Type, Action<MltnBinaryBuilder>>();

        public static byte[] Serialize(object obj)
        {
            var builder = new MltnBinaryBuilder(BodyTypeMapping, HeaderTypeMapping);

            builder.WriteString("data {\r\n");
            builder.Serialize(obj);
            builder.WriteString("}\r\n");

            var data = builder.OutputData();
            var header = builder.OutputHeader();
            // get string table

            return data;
        }

        public static void RegisterMapping(Type type, IMltnBinaryWriter writer)
        {
            BodyTypeMapping.Add(type, writer.WriteBody);
            HeaderTypeMapping.Add(type, writer.WriteHeader);
        }
    }
}
