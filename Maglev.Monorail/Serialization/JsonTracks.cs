using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public static class JsonTracks
    {
        private static readonly Dictionary<Type, Action<object, JsonBuilder>> TypeMapping = new Dictionary<Type, Action<object, JsonBuilder>>();

        public static string Serialize(object obj)
        {
            var jb = new JsonBuilder(TypeMapping);

            jb.Serialize(obj);

            return jb.Output();
        }

        public static void RegisterMapping(Type type, IJsonWriter writer)
        {
            TypeMapping.Add(type, writer.WriteJson);
        }

        public static void ClearMappings()
        {
            TypeMapping.Clear();
        }
    }
}
