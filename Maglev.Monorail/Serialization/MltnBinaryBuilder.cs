using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Maglev.Monorail.Serialization
{
    public enum ControlCode : byte
    {
        MiltonBinaryStartA = 186,
        MiltonBinaryStartB = 190,
        
        StringTableStart = 1,
        HeaderStart = 2,
        BodyStart = 3,

        DefineObject = 64,
        DefineKeys = 65,
        DefineTypeString = DefineType.String,
        DefineTypeInteger = DefineType.Integer,
    }

    public enum DefineType : byte
    {
        String = 66,
        Integer = 67
    }

    internal class MltnStringTable
    {
        private readonly Dictionary<String, Int32> m_Dictionary = new Dictionary<string, int>();
        private readonly List<String> m_Values = new List<string>();

        public int GetIndex(String name)
        {
            int rv = 0;
            if (!m_Dictionary.TryGetValue(name, out rv))
            {
                rv = m_Dictionary.Count;
                m_Dictionary.Add(name, rv);
                m_Values.Add(name);
            }
            return rv;
        }

        public string GetString(int index)
        {
            return m_Values[index];
        }
    }

    public class MltnBinaryBuilder : IDisposable
    {
        private readonly Dictionary<Type, Action<object, MltnBinaryBuilder>> m_DataMapping = new Dictionary<Type, Action<object, MltnBinaryBuilder>>();
        private readonly Dictionary<Type, Action<MltnBinaryBuilder>> m_HeaderMapping;

        private HashSet<Type> m_HeaderCache = new HashSet<Type>();

        private List<String> m_Strings = new List<string>();
        private Dictionary<String,Int32> m_StringIndex = new Dictionary<string, int>();

        private readonly MltnStringTable m_StringTable = new MltnStringTable();
       
        private readonly MemoryStream m_BodyStream;
        private readonly MemoryStream m_HeadStream;

        private readonly BinaryWriter m_BodyWriter;
        private readonly BinaryWriter m_HeadWriter;

        private const int INITAL_STREAM_SIZE = 1024;    // One KiloByte

        public MltnBinaryBuilder(Dictionary<Type, Action<object, MltnBinaryBuilder>> dataMapping,
            Dictionary<Type, Action<MltnBinaryBuilder>> headerMapping)
        {
            m_BodyStream = new MemoryStream(INITAL_STREAM_SIZE);
            m_HeadStream = new MemoryStream(INITAL_STREAM_SIZE);

            m_DataMapping = dataMapping;
            m_HeaderMapping = headerMapping;

            m_BodyWriter = new BinaryWriter(m_BodyStream);
            m_HeadWriter = new BinaryWriter(m_HeadStream);
        }

        public byte[] OutputHeader()
        {
            return m_HeadStream.ToArray();
        }

        public byte[] OutputData()
        {
            return m_BodyStream.ToArray();
        }

        public void Serialize(object o)
        {
            var type = o.GetType();
            if (o is IList)
            {
                // TODO Handle non-generic lists

                var lst = o as IList;

                var genericType = type.GetGenericArguments()[0];
                WriteTypeToHeader(genericType);                
           
                var writer = TryGetMapping(genericType);

                if (lst.Count == 1)
                {
                    writer(lst[0], this);
                }
                else
                {
                    writer(lst[0], this);
                  
                    for (int i = 1; i < lst.Count; i++)
                    {
                        writer(lst[i], this); 
                    }
                }
            }
            else
            {
                // Some other type
                var writer = TryGetMapping(type);
                writer(o, this);
            }
        }

        private void WriteTypeToHeader(Type type)
        {
            if (!m_HeaderCache.Contains(type))
                return;

            Action<MltnBinaryBuilder> writer;
            if (!m_HeaderMapping.TryGetValue(type, out writer))
                throw new Exception(String.Format("Unregistered type:{0} requesting serialisation.", type));

            writer(this);

            m_HeaderCache.Add(type);
        }

        private Action<object, MltnBinaryBuilder> TryGetMapping(Type type)
        {
            Action<object, MltnBinaryBuilder> writer;
            if (!m_DataMapping.TryGetValue(type, out writer))
                throw new Exception(String.Format("Unregistered type:{0} requesting serialisation.", type));
           
            return writer;
        }

        public void WriteProperty(string value)
        {
            var index = m_StringTable.GetIndex(value);
            m_BodyWriter.Write(index);
        }

        public void WriteProperty(Int32 value)
        {
            m_BodyWriter.Write(value);
        }

        public void WriteString(string value)
        {
            var index = m_StringTable.GetIndex(value);
            m_BodyWriter.Write(index);
        }

        public void WriteHeader(string name, DefineType defineType)
        {
            var index = m_StringTable.GetIndex(name);
            m_BodyWriter.Write(index);
            m_BodyWriter.Write((byte)defineType);
        }

        public void WriteControlCode(ControlCode value)
        {
            m_BodyWriter.Write((byte)value);
        }

        public void Dispose()
        {
            m_BodyStream.Dispose();
            m_HeadStream.Dispose();
            m_BodyWriter.Dispose();
            m_HeadWriter.Dispose();
        }
    }
}
