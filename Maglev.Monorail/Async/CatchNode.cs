using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    class CatchNode : AsyncNode
    {
        private readonly Action<Exception> m_Action;

        public CatchNode(Action<Exception> action)
        {
            m_Action = action;
        }

        public void Handle(Exception exception)
        {
            m_Action.Invoke(exception);
        }
    }
}
