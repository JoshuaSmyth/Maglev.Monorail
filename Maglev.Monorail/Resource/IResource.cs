using System;

namespace Maglev.Monorail.Resource
{
    public enum ResourceStatus
    {
        Unloaded = 0,
        Loaded = 1,
        Loading = 2,
        Unloading = 3,
        Failed = 4,
        Active = 5,             // Do not unload this resource
        Inactive = 6,           // Can be unloaded
        AlwaysResident = 7      // Special resource that will never be unloaded (except on quitting the application)
    }

    public interface IResource : IDisposable
    {
        /// The key that uniquely identifes this resource
        string Name { get; }

        // The size of the resource in bytes
        int Size { get; }

        ResourceStatus Status { get; set; }

        IResource Load(string name);

        void Unload();
    }
}
