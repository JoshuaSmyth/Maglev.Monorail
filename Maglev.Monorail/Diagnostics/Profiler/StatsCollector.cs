using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Maglev.Monorail.Diagnostics.Profiler
{
    public static class StatsCollector
    {
        private static class StatsCollectorNodePool 
        {
            const int POOL_SIZE = 16;

            private static readonly ConcurrentQueue<TimerNode> TimeNodePool;

            static StatsCollectorNodePool()
            {
                TimeNodePool = new ConcurrentQueue<TimerNode>();

                for (int i = 0; i < POOL_SIZE; i++)
                {
                    TimeNodePool.Enqueue(new TimerNode());
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void InitPool()
            {
                // Ensures static constructor is called   
            }

            internal static TimerNode RequestItem()
            {
                TimerNode rv;
                if (!TimeNodePool.TryDequeue(out rv))
                {
                    rv = new TimerNode();
                    TimeNodePool.Enqueue(rv);
                }
                return rv;
            }
        }

        public class TimerNode : IDisposable
        {
            private readonly Stopwatch m_Stopwatch;

            public Int32 CallCount;

            public String Name;

            public TimerNode()
            {
                m_Stopwatch = new Stopwatch();
            }

            public void Dispose()
            {
                m_Stopwatch.Stop();
            }

            public void Start()
            {
                m_Stopwatch.Start();
            }

            public void Stop()
            {
                m_Stopwatch.Stop();
            }

            public long ElapsedMilliseconds
            {
                get { return m_Stopwatch.ElapsedMilliseconds; }
            }

            public long ElapsedTicks
            {
                get { return m_Stopwatch.ElapsedTicks; }
            }

            public TimeSpan Elapsed
            {
                get { return m_Stopwatch.Elapsed; }
            }
        }

        private static readonly Dictionary<String, TimerNode> Timings = new Dictionary<string, TimerNode>();

        public static TimerNode Record(string name)
        {
#if ENABLE_STATSCOLLECTOR

            if (!Timings.ContainsKey(name))
            {
                var rv = StatsCollectorNodePool.RequestItem();
                rv.Name = name;
                Timings.Add(name, rv);
                rv.Start();
                return rv;
            }
            else
            {
                var rv = Timings[name];
                rv.CallCount++;
                rv.Start();
                return rv;
            }
#endif
            return null;
        }

        public static Dictionary<String, TimerNode> GetTimings()
        {
            return Timings;
        }

        public static void Init()
        {
#if ENABLE_STATSCOLLECTOR

            StatsCollectorNodePool.InitPool();
            using (StatsCollector.Record("Init"))
            {
                Thread.Sleep(1);
            }
#endif
        }
    }
}
