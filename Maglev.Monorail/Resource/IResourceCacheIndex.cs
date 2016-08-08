namespace Maglev.Monorail.Resource
{
    public interface IResourceCacheIndex
    {
        void AddOrUpdate(int accessTime, string key);
        void Remove(string key);
        void Update();
        bool TryRemoveOldest(out string key);
        void Clear();

        int Count { get; }
    }
}
