using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Serialization
{
    public interface IMltnWriter
    {
        void WriteBody(object input, MltnTextBuilder textBuilder);

        void WriteHeader(MltnTextBuilder textBuilder);
    }
}
