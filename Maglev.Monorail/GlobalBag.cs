using System;
using Maglev.Monorail.Diagnostics.Logging;
using Maglev.Monorail.Graphics;
using Maglev.Monorail.Pak;
using Maglev.Monorail.Resource;

namespace Maglev.Monorail
{
    public static class GlobalBag
    {
        public static IGraphicsDevice GraphicsDevice;

        public static IResourceCache ResourceCache;

        public static IDebugLogger DebugLogger;

        public static IDependancyLocator DependancyLocator;

        public static IPakFileSystem FileSystem;

        public static float Delta { get; private set; }  // The time passed last frame in milliseconds

        public static void Init(IGraphicsDevice graphicsDevice, 
                                IResourceCache resourceCache,
                                IDebugLogger debugLogger,
                                IDependancyLocator dependancyLocator,
                                IPakFileSystem fileSystem)
        {
            GraphicsDevice = graphicsDevice;
            ResourceCache = resourceCache;
            DebugLogger = debugLogger;
            DependancyLocator = dependancyLocator;
            FileSystem = fileSystem;
        }

        // Call once per frame
        public static void Update(float dt)
        {
            Delta = dt;
            ResourceCache.Update();
        }
    }
}
