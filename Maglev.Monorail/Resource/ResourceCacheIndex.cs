using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Maglev.Monorail.Diagnostics.Profiler;

namespace Maglev.Monorail.Resource
{
    // Resource Cache Index First Implementation attempt
    // CPU Cache friendlyness: High
   
    public class ResourceCacheIndex : IResourceCacheIndex
    {
        //private bool m_IsDirty;
        private IndexRecord[] m_Index;

        private int m_TotalSpace;
        private const int PAGE_SIZE = 32;

        public int Count { get; private set; }

        public ResourceCacheIndex(int size = 16)
        {
            m_TotalSpace = size;
            m_Index = new IndexRecord[size];
        }

        public void AddOrUpdate(int accessTime, string key)
        {
            using (StatsCollector.Record("CustomResourceCacheIndex::ReserveSpace"))
            {
                if (Count >= m_TotalSpace)
                {
                    var newSize = Count + PAGE_SIZE;
                    var newIndex = new IndexRecord[newSize];
                    for (var i = 0; i < Count; i++)
                    {
                        newIndex[i] = m_Index[i];
                    }
                    m_Index = newIndex;         // The old index will be GC'd at some stage
                    m_TotalSpace = newSize;
                }
            }

            using (StatsCollector.Record("CustomResourceCacheIndex::AddOrUpdate"))
            {
                // Find Existing Record
                {
                    for (var i = Count - 1; i >= 0; i--)
                    {
                        if (m_Index[i].Key != key)
                            continue;

                        m_Index[i].AccessTime = accessTime;
                        return;
                    }
                }

                // Insert new Record
                {
                    m_Index[Count].Key = key;
                    m_Index[Count].AccessTime = accessTime;
                    Count++;
                }
            }
        }

        public void Remove(string key)
        {
            using (StatsCollector.Record("CustomResourceCacheIndex::Remove"))
            {
                // The index never shrinks, instead we just set the access time to max value so it will end up at the end of the array when sorted.

                for (var i = Count - 1; i >= 0; i--)
                {
                    if (m_Index[i].Key != key) 
                        continue;

                    m_Index[i].AccessTime = int.MaxValue;

                    // Shuffle elements
                    for (var j = i + 1; j < Count; j++)
                    {
                        m_Index[j - 1] = m_Index[j];
                    }
                    Count--;
                    return;
                }
            }
        }

        public void Update()
        {
            // A sort would only be required on bulk inserts or bulk deletes
            // Currently don't do this so no need to sort for now.

            /*
            using (StatsCollector.Record("CustomResourceCacheIndex::Update"))
            {
                if (!m_IsDirty) 
                    return;

                // Insertion Sort
                {
                    var length = Count;

                    for (var i = 1; i < length; i++)
                    {
                        var j = i;

                        while ((j > 0) && (m_Index[j].AccessTime < m_Index[j - 1].AccessTime))
                        {
                            var k = j - 1;
                            var temp = m_Index[k];
                            m_Index[k] = m_Index[j];
                            m_Index[j] = temp;

                            j--;
                        }
                    }
                }

                m_IsDirty = false;
            }
             */
        }

        public bool TryRemoveOldest(out string key)
        {
            using (StatsCollector.Record("ResourceCacheIndex:TryRemoveOldest"))
            {
                // The index never shrinks, instead we just set the access time to max value so it will end up at the end of the array when sorted.

                key = m_Index[0].Key;
                m_Index[0].AccessTime = int.MaxValue;

                // Shuffle elements
                for (var i = 1; i < Count; i++)
                {
                    m_Index[i - 1] = m_Index[i];
                }

                Count--;
                return true;
            }
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}
