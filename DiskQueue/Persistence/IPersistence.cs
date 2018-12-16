using System;
using System.Collections.Generic;

namespace PersistedQueue.Persistence
{
    public interface IPersistence<T> : IDisposable
    {
        // TODO: summary comments
        IEnumerable<T> Load();
        T Load(uint key);
        void Persist(uint key, T item);
        void Remove(uint key);
        void Clear();
    }
}
