using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersistedQueue.Persistence
{
    public interface IPersistence<T> : IDisposable
    {
        /// <summary>
        /// Load all items from persistence
        /// </summary>
        /// <returns>The loaded items</returns>
        IEnumerable<T> Load();

        /// <summary>
        /// Load an item with the specified key
        /// </summary>
        /// <returns>The loaded item</returns>
        /// <param name="key">The key to load</param>
        T Load(uint key);

        /// <summary>
        /// Loads an item with the specified key asynchronously
        /// </summary>
        /// <returns>The item load task</returns>
        /// <param name="key">The key to load</param>
        Task<T> LoadAsync(uint key);

        /// <summary>
        /// Persist the specified key and item.
        /// </summary>
        /// <param name="key">The key to persist the item under</param>
        /// <param name="item">The item to persist</param>
        void Persist(uint key, T item);

        /// <summary>
        /// Remove the specified key.
        /// </summary>
        /// <param name="key">The key to remove</param>
        void Remove(uint key);

        /// <summary>
        /// Clear this instance.
        /// </summary>
        void Clear();
    }
}
