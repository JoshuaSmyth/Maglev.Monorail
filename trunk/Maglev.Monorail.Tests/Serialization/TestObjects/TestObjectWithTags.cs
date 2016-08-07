using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maglev.Monorail.Conversion;

namespace Maglev.Monorail.Tests.Serialization.TestObjects
{
    public class TestObjectWithTags
    {
        public String Name { get; set; }

        public Int32 Age { get; set; }

        public Int32 Balance { get; set; }

         public List<String> Tags { get; set; } 

        public TestObjectWithTags()
        {
            Tags = new List<string>();
        }

        public void Serialise(StringBuilder builder)
        {
            builder.Append("\"Name\":\"");
            builder.Append(Name);
            builder.Append("\",\"Age\":");
            builder.Append(IntUtils.IntToStr(this.Age));
            builder.Append(",\"Balance\":");
            builder.Append(IntUtils.IntToStr(this.Balance));

            builder.Append(",\"Tags\":[");
            foreach (var tag in Tags)
            {
                builder.Append('"');
                builder.Append(tag);
                builder.Append('"');
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);  // Is this faster than a branch?
            builder.Append("]");
        }

        public void Serialise2(StringBuilder builder)
        {
            builder.Append("\"Name\":\"");
            builder.Append(Name);
            builder.Append("\",\"Age\":");
            builder.Append(this.Age);
            builder.Append(",\"Balance\":");
            builder.Append(this.Balance);
        }
    }
}
