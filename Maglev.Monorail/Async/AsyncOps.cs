using System;
using System.Collections;
using System.Collections.Generic;

namespace Maglev.Monorail.Async
{
    public class AsyncOps
    {
        private readonly List<AsyncNode> m_ActiveNodes = new List<AsyncNode>(8);

        public AsyncNode Queue(Action action)
        {
            var rv = new AsyncNode(action);
            m_ActiveNodes.Add(rv);
            return rv;
        }

        public AsyncNode Queue(AsyncNode node)
        {
            m_ActiveNodes.Add(node);
            return node;
        }

        public AsyncNode Queue(IEnumerator enumerator)
        {
            var rv = new CoroutineNode(enumerator);
            m_ActiveNodes.Add(rv);
            return rv;
        }

        public virtual void Update(Int32 dt)
        {
            for (int i = 0; i < m_ActiveNodes.Count; i++)
            {
                m_ActiveNodes[i].Update(dt);
            }

            for (int i = 0; i < m_ActiveNodes.Count; i++)
            {
                var node = m_ActiveNodes[i];

                if (node.Status == AsyncStatus.ThrewException)
                {
                    m_ActiveNodes.Clear();
                    return;
                }

                if (node.Status == AsyncStatus.Finished ||
                    node.Status == AsyncStatus.Ignore)
                {
                    m_ActiveNodes.Remove(node);
                    m_ActiveNodes.AddRange(node.Children);
                    i--;
                }
            }
        }

        public bool HasActions()
        {
            return m_ActiveNodes.Count > 0;
        }


        public void Cancel(AsyncNode node)
        {
            throw new NotImplementedException();
        }
    }
}
