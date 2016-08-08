using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public static class MltnTextTracks
    {
        private static readonly Dictionary<Type, Action<object, MltnTextBuilder>> BodyTypeMapping = new Dictionary<Type, Action<object, MltnTextBuilder>>();
        private static readonly Dictionary<Type, Action<MltnTextBuilder>> HeaderTypeMapping = new Dictionary<Type, Action<MltnTextBuilder>>();

        public static string Serialize(object obj, PrintMode printMode=PrintMode.Pretty)
        {
            var builder = new MltnTextBuilder(BodyTypeMapping, HeaderTypeMapping, printMode);

            builder.WriteString("data {\r\n");
            builder.SpaceIndex++;
            builder.Serialize(obj);
            builder.SpaceIndex--;
            builder.WriteString("}\r\n");

            var data = builder.OutputData();
            var header = builder.OutputHeader();

            var sb = new StringBuilder();
            sb.Append("header {\r\n");
            sb.Append(header);
            sb.Append("}\r\n");
            sb.Append(data);
           
            return sb.ToString();
        }

        public static void RegisterMapping(Type type, IMltnWriter writer)
        {
            BodyTypeMapping.Add(type, writer.WriteBody);
            HeaderTypeMapping.Add(type, writer.WriteHeader);
        }

        public static void ClearMappings()
        {
            BodyTypeMapping.Clear();
            HeaderTypeMapping.Clear();
        }
    }
}
