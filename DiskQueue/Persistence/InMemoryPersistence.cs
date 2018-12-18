using System;
using System.Collections.Generic;

namespace PersistedQueue.Persistence
{
    public class InMemoryPersistence<T> : IPersistence<T>
    {
        Dictionary<uint, T> items = new Dictionary<uint, T>();

        public T Load(uint key)
        {
            return items[key];
        }

        public void Persist(uint key, T item)
        {
            items[key] = item;
        }

        public void Remove(uint key)
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

        public void Dispose()
        {
        }
    }
}
