using System.Collections.Generic;

namespace PersistedQueue
{
    public interface IPersistence<T>
    {
        // TODO: summary comments
        IEnumerable<T> Load();
        T Load(long key);
        void Persist(long key, T item);
        void Remove(long key);
        void Clear();
    }
}
