using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Conversion;

namespace Maglev.Monorail.Serialization
{
    public class MltnTextBuilder
    {
        private readonly Dictionary<Type, Action<object, MltnTextBuilder>> m_DataMapping = new Dictionary<Type, Action<object, MltnTextBuilder>>();
        private readonly Dictionary<Type, Action<MltnTextBuilder>> m_HeaderMapping;

        private HashSet<Type> m_HeaderCache = new HashSet<Type>();

        private readonly PrintMode m_PrintMode;

        private StringBuilder m_Hsb; // = new StringBuilder();

        private StringBuilder m_Sb; // = new StringBuilder();

        public MltnTextBuilder(Dictionary<Type, Action<object, MltnTextBuilder>> dataMapping, 
            Dictionary<Type, Action<MltnTextBuilder>> headerMapping,
            PrintMode printMode)
        {
            m_DataMapping = dataMapping;
            m_HeaderMapping = headerMapping;
            m_PrintMode = printMode;

            m_Hsb = new StringBuilder(128); //PooledStringBuilder.Request();
            m_Sb = new StringBuilder(128); //PooledStringBuilder.Request();
        }

        internal int SpaceIndex
        {
            get { return m_SpaceIndex; }
            set { m_SpaceIndex = value; }
        }

        public string OutputHeader()
        {
            return m_Hsb.ToString();
        }

        public string OutputData()
        {
            return m_Sb.ToString();
        }

        private Int32 m_SpaceIndex = 0;
        private static readonly string[] spaceBuffer = new string[]
            {
                "",                     // 0
                "  ",                   // 1
                "    ",         
                "      ",
                "        ",
                "          ",
                "            ",
                "              ",
                "                ",
            };

        private void AppendSpaces()
        {
            if (m_PrintMode == PrintMode.Compact)
                return;

            if (SpaceIndex >= spaceBuffer.Count())
            {
                var lastIndex = spaceBuffer.Length - 1;
                m_Sb.Append(spaceBuffer[lastIndex]);

                var tmp = SpaceIndex - lastIndex;
                for (var i = 0; i < tmp; i++)
                {
                    m_Sb.Append(" ");
                }
            }
            else
            {
                m_Sb.Append(spaceBuffer[SpaceIndex]);
            }
        }

        private void AppendLine()
        {
            if (m_PrintMode == PrintMode.Compact)
            {
                m_Sb.Append("\r");
                return;
            }
            m_Sb.Append("\r\n");
        }

        private void AppendLine(string value)
        {
            m_Sb.Append(value);
            if (m_PrintMode == PrintMode.Compact)
                return;
          
            m_Sb.Append("\r\n");
        }

        public void Serialize(object o)
        {
            var type = o.GetType();
            if (o is IList)
            {
                var lst = o as IList;

                WriteArray(type, lst);
            }
            else
            {
                if (type == typeof (string) || type == typeof (String))
                {
                    WriteString(o as string);
                }
                else
                {
                    // Some other type
                    var writer = TryGetMapping(type);
                    writer(o, this);
                }
            }
        }

        private void WriteArray(Type type, IList lst)
        {
            var genericType = type.GetGenericArguments()[0];
            WriteTypeToHeader(genericType);
            // TODO Handle non-generic lists

            AppendSpaces();
            m_Sb.Append("Array<");
            m_Sb.Append(genericType.Name);
            m_Sb.Append(">(");
            m_Sb.Append(lst.Count);
            AppendLine(") [");

            var writer = TryGetMapping(genericType);

            if (lst.Count == 1)
            {
                SpaceIndex++;
                AppendSpaces();
                AppendLine("{");
                SpaceIndex++;
                writer(lst[0], this);
                SpaceIndex--;
                AppendSpaces();
                AppendLine("}");
                SpaceIndex--;
            }
            else
            {
                SpaceIndex++;
                AppendSpaces();
                AppendLine("{");
                SpaceIndex++;
                writer(lst[0], this);
                SpaceIndex--;
                for (int i = 1; i < lst.Count; i++)
                {
                    m_Sb.Append("\r\n");
                    AppendSpaces();
                    AppendLine("}");

                    AppendSpaces();
                    AppendLine("{");

                    SpaceIndex++;
                    writer(lst[i], this);
                    SpaceIndex--;
                }
                AppendSpaces();
                m_Sb.Append("}");
                SpaceIndex--;
            }
            m_Sb.Append("]");
        }

        private void WriteTypeToHeader(Type type)
        {
           if (m_HeaderCache.Contains(type))
                return;

            Action<MltnTextBuilder> writer;
            if (!m_HeaderMapping.TryGetValue(type, out writer))
                throw new Exception(String.Format("Unregistered type:{0} requesting serialisation.", type));

            m_Hsb.Append("  ");
            m_Hsb.Append(type.Name);
            m_Hsb.Append("\r\n");
            m_Hsb.Append("  {\r\n");
            writer(this);
            m_Hsb.Append("  }\r\n");

            m_HeaderCache.Add(type);
        }

        private Action<object, MltnTextBuilder> TryGetMapping(Type type)
        {
            Action<object, MltnTextBuilder> writer;
            if (!m_DataMapping.TryGetValue(type, out writer))
                throw new Exception(String.Format("Unregistered type:{0} requesting serialisation.", type));
           
            return writer;
        }

        public void WriteProperty(string value)
        {
            AppendSpaces();
            m_Sb.Append("\"");
            m_Sb.Append(value);
            m_Sb.Append("\"");
            AppendLine();
        }

        public void WriteProperty(Int32 value)
        {
            AppendSpaces();
            m_Sb.Append(IntUtils.IntToStr(value));
            AppendLine();
        }

        public void WriteString(string data)
        {
            m_Sb.Append(data);
        }

        public void WriteHeader(string name, string type)
        {
            m_Hsb.Append("    ");
            m_Hsb.Append(name);
            m_Hsb.Append(":");
            m_Hsb.Append(type);
            m_Hsb.Append("\r\n");
        }

        // TODO Write object array

        public void WriteStringArray(IList<String> lst)
        {
            //return;

            AppendSpaces();
            m_Sb.Append("(");
            m_Sb.Append(lst.Count);
            m_Sb.Append(")[");

            if (lst.Count == 1)
            {
                m_Sb.Append("\"");
                m_Sb.Append(lst[0]);
                m_Sb.Append("\"]");
            }
            else
            {
                m_Sb.Append("\"");
                m_Sb.Append(lst[0]);
                for (int i = 1; i < lst.Count; i++)
                {
                    m_Sb.Append("\",\"");
                    m_Sb.Append(lst[i]);
                }
                m_Sb.Append("\"]");
            }
        }
    }
}
