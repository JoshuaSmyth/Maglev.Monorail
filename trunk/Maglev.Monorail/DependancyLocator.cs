using System;
using System.Collections.Generic;
using Maglev.Monorail.Resource;

namespace Maglev.Monorail
{
    public class DependancyLocator : IDependancyLocator
    {
        readonly Dictionary<Type, object> m_Map;

        public DependancyLocator()
        {
            m_Map = new Dictionary<Type, object>();
        }
 
        public void AddDepenancy<T>(T obj)
        {
            var type = typeof (T);
            if (!m_Map.ContainsKey(type)) 
            {
                  m_Map.Add(type, obj);
            }
            else
            {
                throw new ApplicationException("The type " + type.FullName + " is not registered");
            }
        }
          
        public void ClearAll()
        {
            m_Map.Clear();
        }

        public T GetDependancy<T>() where T : class
        {
            var type = typeof(T);
            if (m_Map.ContainsKey(type))
            {
                return m_Map[type] as T;
            }
            throw new ApplicationException("The type " + type.FullName + " is not registered");
        }
    }
}
