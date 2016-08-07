using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Maglev.Monorail.Diagnostics.Logging;
using Maglev.Monorail.Diagnostics.Profiler;
using Maglev.Monorail.Resource;
using Maglev.Monorail.Tests.Mocks;
using NUnit.Framework;

namespace Maglev.Monorail.Tests
{
    [TestFixture]
    public class ResourceTests
    {
        public const Int32 ONE_MEGABYTE = 1048576;
        public const Int32 ONE_KILOBYTE = 1024;

        public ResourceTests()
        {
            StatsCollector.Init();
        }

        [Test]
        public void TestShouldPass()
        {
            Assert.Pass();
        }

        [Test]
        public void TestLoadResources()
        {
            using (var resourceCache = new ResourceCache(ONE_MEGABYTE, new ResourceCacheIndex(16), new ResourceLoader()))
            {
                for (int i = 0; i < 1025; i++)
                {
                    IResource myResource = new TestResource("name" + i);
                    var success = resourceCache.TryLoadResource(myResource, out myResource);
                    Assert.IsTrue(success == ResourceStatus.Loaded);
                }
            }
        }

        [Test]
        public void TestLoadResources_StartUnloading()
        {
            using (var resourceCache = new ResourceCache(ONE_MEGABYTE, new ResourceCacheIndex(16), new ResourceLoader()))
            {
                var debugLogger = new DebugLogger();

                // TODO(Joshua) just directly write to the GlobalBag.DebugLogger
                /*
                resourceCache.OnLoad += ((obj, args) => debugLogger.Log(LogLevel.Info, "ResourceCache", args.MyEventString ));
                resourceCache.OnUnload += ((obj, args) => debugLogger.Log(LogLevel.Info, "ResourceCache", args.MyEventString));
                resourceCache.OnCacheHit += ((obj, args) => debugLogger.Log(LogLevel.Info, "ResourceCache", args.MyEventString));
                */
                for (int i = 1; i < 8192*2; i++)
                {
                    IResource myResource = new TestResource("name" + i, size:1096*i);
                    var success = resourceCache.TryLoadResource(myResource, out myResource);
                    var cacheHit = resourceCache.TryLoadResource(myResource, out myResource);

                    IResource prevResource = new TestResource("name" + i, size: 1096 * i);
                    var cacheHit1 = resourceCache.TryLoadResource(prevResource, out prevResource);

                    IResource myResource2 = new TestResource("name2" + i, size:1092*i);
                    var success2 = resourceCache.TryLoadResource(myResource2, out myResource);
                  
                    resourceCache.Update();
                }
            }

            var timings = StatsCollector.GetTimings();
            var totalTicks = timings.Values.Sum(o => o.ElapsedTicks);
            var totalMs = timings.Values.Sum(o => o.ElapsedMilliseconds);
            Trace.WriteLine(String.Format("Total ticks:{0}", totalTicks));
            Trace.WriteLine(String.Format("Total ms:{0}", totalMs));
        }
    }
}
