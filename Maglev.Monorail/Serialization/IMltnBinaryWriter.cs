using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public interface IMltnBinaryWriter
    {
        void WriteBody(object input, MltnBinaryBuilder builder);

        void WriteHeader(MltnBinaryBuilder builder);
    }
}
