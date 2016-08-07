using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    class ParallelNode : AsyncNode
    {
        public ParallelNode(Action action) : base(action)
        {
        }
    }
}
