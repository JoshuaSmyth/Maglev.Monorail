using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    class SequenceNode : AsyncNode
    {
        public SequenceNode(Action action) : base(action)
        {
        }
    }
}
