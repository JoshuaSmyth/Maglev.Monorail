using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public interface IJsonWriter
    {
        void WriteJson(object input, JsonBuilder jb);
    }
}
