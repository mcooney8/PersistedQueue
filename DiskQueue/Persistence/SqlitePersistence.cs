using System;
using System.Collections.Generic;

namespace PersistedQueue.Persistence
{
    public class SqlitePersistence<T> : IPersistence<T>
    {
        public SqlitePersistence(string filename)
        {
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public T Load(long key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Load()
        {
            throw new NotImplementedException();
        }

        public void Persist(long key, T item)
        {
            throw new NotImplementedException();
        }

        public void Remove(long key)
        {
            throw new NotImplementedException();
        }
    }
}
