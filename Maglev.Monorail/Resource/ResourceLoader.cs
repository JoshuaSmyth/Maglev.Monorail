using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Maglev.Monorail.Resource
{
    public class ResourceLoader : IResourceLoader
    {
        public IResource QueueLoadResource(IResource resource)
        {
            resource.Status = ResourceStatus.Loading;

            ThreadPool.QueueUserWorkItem(o =>           // TODO (Joshua) Use a Balanced scheduler?
                {
                    try
                    {
                        resource.Load(resource.Name);
                        resource.Status = ResourceStatus.Loaded;
                    }
                    catch (Exception exception)
                    {
                        GlobalBag.DebugLogger.LogException(exception);
                        throw;
                    }
                });

            return resource;
        }

        public void Update()
        {
          
        }
    }
}
