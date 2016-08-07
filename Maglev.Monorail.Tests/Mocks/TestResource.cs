using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Maglev.Monorail.Resource;

namespace Maglev.Monorail.Tests.Mocks
{
    public class TestResource : IResource
    {
        public TestResource(String name, Int32 size=1024)
        {
            Name = name;
            Size = size;
        }

        public void Dispose()
        {

        }

        public string Name { get; private set; }

        public int Size { get; private set; }

        public ResourceStatus Status { get; set; }

        public IResource Load(string name)
        {
            Name = name;
            //Thread.Sleep(1);
            //Size = 1024;
            Status = ResourceStatus.Loaded;
            return this;
        }

        public void Unload()
        {
            Status = ResourceStatus.Loading;
            Dispose();
            Status = ResourceStatus.Unloaded;
        }
    }
}
