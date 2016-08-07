using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    public class WaitNode : AsyncNode
    {
        private readonly float m_Waittime;
        private float m_CurrentTime;
        public WaitNode(Action action, Int32 waittime) : base(action)
        {
            m_Waittime = waittime;
        }

        public override void Update(float dt)
        {
            if (this.Status == AsyncStatus.Queued)
            {
                this.Status = AsyncStatus.Running;
            }

            m_CurrentTime += dt;
            if (m_CurrentTime >= m_Waittime)
            {
                this.Status = AsyncStatus.Finished;
            }
        }
    }
}
