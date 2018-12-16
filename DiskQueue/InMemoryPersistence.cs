using System.Collections.Generic;

namespace PersistedQueue
{
    public class InMemoryPersistence<T> : IPersistence<T>
    {
        Dictionary<long, T> items = new Dictionary<long, T>();

        public T Load(long key)
        {
            return items[key];
        }

        public void Persist(long key, T item)
        {
            items[key] = item;
        }

        public void Remove(long key)
        {
            items.Remove(key);
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerable<T> Load()
        {
            return new List<T>();
        }
    }
}
