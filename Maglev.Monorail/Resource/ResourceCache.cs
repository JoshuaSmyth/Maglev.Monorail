using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maglev.Monorail.Resource
{
    public class ResourceCache : IResourceCache
    {
        private readonly IResourceCacheIndex m_CacheIndexStrategyIndex; // = new ResourceCacheIndex(16);

        private readonly IResourceLoader m_ResourceLoader;

        private readonly Dictionary<string, IResource> m_Cache = new Dictionary<string, IResource>();
     
        public int CurrentSize { get; set; }

        public int MaxSize { get; set; }

        public int CriticalSize { get; set; }

        private int m_AccessId;

        public ResourceCache(Int32 cacheSizeInBytes,
                             IResourceCacheIndex cacheIndexStrategy,
                             IResourceLoader resourceLoader)
        {
            MaxSize = cacheSizeInBytes;
            CriticalSize = cacheSizeInBytes * 2;

            m_CacheIndexStrategyIndex = cacheIndexStrategy;
            m_ResourceLoader = resourceLoader;
            AssertCacheSize(cacheSizeInBytes);
        }

        public ResourceStatus TryLoadResource(IResource resource, out IResource loadedResource)
        {
            var accessTime = Interlocked.Increment(ref m_AccessId);

            var name = resource.Name;
            if (CurrentSize < CriticalSize)
            {
                IResource rv;
                if (m_Cache.TryGetValue(name, out rv))
                {
                    m_CacheIndexStrategyIndex.AddOrUpdate(accessTime, name);
                    loadedResource = rv;

                    LogCacheHit();
                    return ResourceStatus.Loaded;
                }

                m_ResourceLoader.QueueLoadResource(resource);
                
                AssertResourceChecks(resource);

                LogLoad();

                m_Cache.Add(name, resource);
                m_CacheIndexStrategyIndex.AddOrUpdate(accessTime, name);

                CurrentSize += resource.Size;
                loadedResource = resource;
                return ResourceStatus.Loaded;
            }

            loadedResource = null;
            return ResourceStatus.Failed;
        }
        public List<IResource> LoadedResources
        {
            get { return m_Cache.Values.ToList(); }
        }

        public void UnloadResource(string name)
        {
            IResource rv;
            if (m_Cache.TryGetValue(name, out rv))
            {
                m_CacheIndexStrategyIndex.Remove(name);
                m_Cache.Remove(name);
                CurrentSize -= rv.Size;
                rv.Dispose();

                LogUnload();
            }
        }

        public void UnloadAll()
        {
            foreach (var resource in m_Cache.Values)
            {
                resource.Dispose();
                LogUnload();
            }

            m_CacheIndexStrategyIndex.Clear();
            m_Cache.Clear();
            CurrentSize = 0;
        }

        public void Update()
        {
            m_CacheIndexStrategyIndex.Update();

            if (CurrentSize > CriticalSize)
            {
                UnloadAll();
            }
            
            while(CurrentSize > MaxSize)
            {
                string key;
                if (m_CacheIndexStrategyIndex.TryRemoveOldest(out key))
                {
                    var item = m_Cache[key];
                    m_Cache.Remove(key);
                    CurrentSize -= item.Size;
                    item.Dispose();

                    LogUnload();
                }
                else
                {
                    // Should never happen, so we throw an exception
                    throw new ApplicationException("Fatal Error with the resource cache");
                }
            }
        }

        public void Dispose()
        {
            UnloadAll();
        }

#if DEBUG
        public event EventHandler<DebugLogEventArgs> OnCacheHit;

        public event EventHandler<DebugLogEventArgs> OnUnload;

        public event EventHandler<DebugLogEventArgs> OnLoad;

        private void LogUnload()
        {
            if (OnUnload != null)
                OnUnload.Invoke(this, new DebugLogEventArgs("Resource Unloaded"));
        }

        private void LogLoad()
        {
            if (OnLoad != null)
                OnLoad.Invoke(this, new DebugLogEventArgs("Resource Loaded"));
        }

        private void LogCacheHit()
        {
            if (OnCacheHit != null)
                OnCacheHit.Invoke(this, new DebugLogEventArgs("Resource Cache Hit"));
        }

        private static void AssertResourceChecks(IResource resource)
        {
            if (resource.Size < 0)
                throw new ArgumentOutOfRangeException("Size of resource must be non-zero");

            if (String.IsNullOrEmpty(resource.Name))
                throw new ArgumentOutOfRangeException("Name of resource cannot be null or empty");
        }

        private static void AssertCacheSize(int maxSize)
        {
            if (maxSize < 4096)
                throw new ArgumentOutOfRangeException("maxSize should be greater than 4kb");
        }
#else
        private static void LogUnload() { }

        private static void LogLoad() { }

        private static void LogCacheHit() { }

        private static void AssertResourceChecks(IResource resource) { }

        private static void AssertCacheSize(int maxSize) { }
#endif
    }
}
