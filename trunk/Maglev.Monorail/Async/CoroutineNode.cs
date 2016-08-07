using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    public class CoroutineNode : AsyncNode
    {
        private readonly IEnumerator m_Enumerable;

        public CoroutineNode(IEnumerator enumerable)
        {
            m_Enumerable = enumerable;
        }


        public override void Update(float dt)
        {
            if (this.Status == AsyncStatus.Queued)
            {
                this.Status = AsyncStatus.Running;
            }


            if (!MoveNext(m_Enumerable))
            {
                this.Status = AsyncStatus.Finished;
            }
        }

        bool MoveNext(IEnumerator routine)
        {
            var current = routine.Current as IEnumerator;
            if (current != null)
            {
                if (MoveNext(current))
                    return true;
            }

            return routine.MoveNext();
        }
    }
}
