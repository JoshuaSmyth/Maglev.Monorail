using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Conversion;

namespace Maglev.Monorail.Serialization
{
    public class JsonBuilder
    {
        private Dictionary<Type, Action<object, JsonBuilder>> m_TypeMapping = new Dictionary<Type, Action<object, JsonBuilder>>();
        
        StringBuilder m_Sb = new StringBuilder();
        public JsonBuilder(Dictionary<Type, Action<object, JsonBuilder>> typeMapping)
        {
            m_TypeMapping = typeMapping;
        }

        public string Output()
        {
            return m_Sb.ToString();
        }

        private static Int32 spaceIndex = 0;
        private static string[] spaceBuffer = new string[]
            {
                "",             // 0
                " ",            // 1
                "  ",           // 2
                "    "          // 3    (4 spaces)
            };

        private void AppendSpaces()
        {
            if (spaceIndex >= spaceBuffer.Count())
                throw new NotImplementedException("Increase the size of the spaces buffer");

            m_Sb.Append(spaceBuffer[spaceIndex]);
        }

        public void Serialize(object o)
        {
            
            var type = o.GetType();
            if (o is IList)
            {
                // TODO Handle non-generic lists
                AppendSpaces(); m_Sb.Append("[\r\n");
                spaceIndex++;

                var genericType = type.GetGenericArguments()[0];
                var writer = TryGetMapping(genericType);

                var lst = o as IList;
                if (lst.Count == 1)
                {
                    spaceIndex++;
                    AppendSpaces(); m_Sb.Append("{\r\n");
                    spaceIndex++;
                    writer(lst[0], this);
                    spaceIndex--;
                    AppendSpaces(); m_Sb.Append("}\r\n");
                    spaceIndex--;
                }
                else
                {
                    spaceIndex++;
                    AppendSpaces(); m_Sb.Append("{\r\n");
                    spaceIndex++;
                    writer(lst[0], this);
                    spaceIndex--;
                    for (int i = 1; i < lst.Count; i++)
                    {
                        AppendSpaces();
                        m_Sb.Append("},\r\n");

                        AppendSpaces();
                        m_Sb.Append("{\r\n");

                        spaceIndex++;
                        writer(lst[i], this);
                        spaceIndex--;
                    }
                    AppendSpaces(); m_Sb.Append("}\r\n");
                    spaceIndex--;
                }
                spaceIndex--;
                m_Sb.Append("]");
            }
            else
            {
                // Some other type
                var writer = TryGetMapping(type);
                writer(o, this);
            }
        }

        private Action<object, JsonBuilder> TryGetMapping(Type type)
        {
            Action<object, JsonBuilder> writer;
            if (!m_TypeMapping.TryGetValue(type, out writer))
                throw new Exception(String.Format("Unregistered type:{0} requesting serialisation.", type));
            return writer;
        }

        public void WriteProperty(string name, string value)
        {
            AppendSpaces();
            m_Sb.Append("\"");
            m_Sb.Append(name);
            m_Sb.Append("\": ");
            m_Sb.Append(value);
            m_Sb.Append("\r\n");
        }

        public void WriteProperty(string name, Int32 value)
        {
            AppendSpaces();
            m_Sb.Append("\"");
            m_Sb.Append(name);
            m_Sb.Append("\": ");
            m_Sb.Append(IntUtils.IntToStr(value));
            m_Sb.Append("\r\n");
        }

        public void WritePropertyC(string name, string value)
        {
            AppendSpaces();
            m_Sb.Append("\"");
            m_Sb.Append(name);
            m_Sb.Append("\": \"");
            m_Sb.Append(value);
            m_Sb.Append("\",");
            m_Sb.Append("\r\n");
        }

        public void WritePropertyC(string name, Int32 value)
        {
            AppendSpaces();
            m_Sb.Append("\"");
            m_Sb.Append(name);
            m_Sb.Append("\": ");
            m_Sb.Append(IntUtils.IntToStr(value));
            m_Sb.Append(",");
            m_Sb.Append("\r\n");
        }
    }
}
