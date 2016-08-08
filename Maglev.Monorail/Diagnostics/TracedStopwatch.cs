using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Maglev.Monorail.Diagnostics
{
    public class TracedStopwatch : IDisposable
    {
        private static class TracedStopwatchPool
        {
            private const int STARTING_POOL_SIZE = 16;

            private static readonly Queue<TracedStopwatch> Pool;

            static TracedStopwatchPool()
            {
                Pool = new Queue<TracedStopwatch>(STARTING_POOL_SIZE);
                
                for (int i = 0; i < STARTING_POOL_SIZE; i++)
                {
                    Pool.Enqueue(new TracedStopwatch());
                }
            }

             [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void Init()
            {
                // Ensures static constructor is called
            }

            internal static TracedStopwatch Request()
            {
                lock (Pool)
                {
                    if (Pool.Count == 0)
                    {
                        Pool.Enqueue(new TracedStopwatch());
                    }

                    return Pool.Dequeue();
                }
            }

            internal static void Return(TracedStopwatch item)
            {
                 lock (Pool)
                 {
                     Pool.Enqueue(item);
                 }
            }
        }

        private readonly Stopwatch m_Stopwatch;
        private string m_Name;

        private TracedStopwatch()
        {
            m_Stopwatch = new Stopwatch();
        }

        public void Dispose()
        {
            m_Stopwatch.Stop();
            var resultString = String.Format("TracedStopwatch:{0} Total Milliseconds:{1}", m_Name, m_Stopwatch.ElapsedMilliseconds);
            Trace.WriteLine(resultString);

            TracedStopwatchPool.Return(this);
        }

        public static TracedStopwatch Create(String name)
        {
            var rv = TracedStopwatchPool.Request();
            rv.m_Name = name;
            rv.m_Stopwatch.Restart();
            return rv;
        }

        public static void Init()
        {
            TracedStopwatchPool.Init();
        }
    }
}
