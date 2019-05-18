using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistedQueue.Persistence
{
    public class InMemoryPersistence<T> : IPersistence<T>
    {
        ConcurrentDictionary<uint, T> items = new ConcurrentDictionary<uint, T>();

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
            items.TryRemove(key, out _);
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

        public Task<T> LoadAsync(uint key)
        {
            return Task.Run(() => Load(key));
        }
    }
}
