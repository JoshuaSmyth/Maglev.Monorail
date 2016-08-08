using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Resource
{
    public interface IResourceLoader
    {
        IResource QueueLoadResource(IResource resource);
    }
}
