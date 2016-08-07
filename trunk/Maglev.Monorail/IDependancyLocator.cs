namespace Maglev.Monorail.Resource
{
    public interface IDependancyLocator
    {
        void AddDepenancy<T>(T obj);

        T GetDependancy<T>() where T : class;

        void ClearAll();
    }
}
