using System;
using System.Collections.Generic;

namespace Maglev.Monorail.Resource
{
    public interface IResourceCache : IDisposable
    {
        // The current size of the cache in bytes
        Int32 CurrentSize   { get; set; }
        
        // The Size at which the resource cache will start to purge
        Int32 MaxSize       { get; set; }

        // The Size at which the resource cache will start to agressively purge
        Int32 CriticalSize  { get; set; }

        void Update();

        ResourceStatus TryLoadResource(IResource resource, out IResource loadedResource);

        List<IResource> LoadedResources { get; }

#if DEBUG
        event EventHandler<DebugLogEventArgs> OnCacheHit;

        event EventHandler<DebugLogEventArgs> OnUnload;

        event EventHandler<DebugLogEventArgs> OnLoad;
#endif
    }
}
