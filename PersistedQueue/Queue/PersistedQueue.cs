using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersistedQueue.Persistence;

namespace PersistedQueue
{
    /// <summary>
    /// Persisted queue is a queue implementation that also keeps either some or all 
    /// items on disk. It is a thread-safe implementation.
    /// </summary>
    public class PersistedQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly IPersistence<T> persistence;
        private readonly int maxItemsInMemory;
        private readonly FixedArrayQueue<Task<T>> inMemoryItems;
        private readonly SemaphoreSlim queueSemaphore = new SemaphoreSlim(1);
        private readonly bool persistAllItems;

        private uint nextKey;
        private uint firstKey = 1;

        private uint persistenceFirstKey = uint.MaxValue;

        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PersistedQueue.PersistedQueue`1"/> class.
        /// </summary>
        /// <param name="persistence"></param>
        public PersistedQueue(IPersistence<T> persistence) :
            this(persistence, new PersistedQueueConfiguration { DeferLoad = false, MaxItemsInMemory = 1024, PersistAllItems = true })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PersistedQueue.PersistedQueue`1"/> class.
        /// </summary>
        /// <param name="persistence"></param>
        /// <param name="configuration"></param>
        public PersistedQueue(IPersistence<T> persistence, PersistedQueueConfiguration configuration)
        {
            this.persistence = persistence;
            persistAllItems = configuration.PersistAllItems;
            maxItemsInMemory = configuration.MaxItemsInMemory;
            if (maxItemsInMemory < 1)
            {
                throw new ArgumentException("Must be greater than 0", nameof(maxItemsInMemory));
            }
            inMemoryItems = new FixedArrayQueue<Task<T>>(maxItemsInMemory);
            if (!configuration.DeferLoad)
            {
                Load();
            }
        }

        /// <summary>
        /// Gets the count of items
        /// </summary>
        /// <value>The count of items</value>
        public int Count { get; private set; }

        /// <summary>
        /// Enqueue the specified item.
        /// </summary>
        /// <param name="item">Item to be enqueued</param>
        public void Enqueue(T item)
        {
            queueSemaphore.Wait();
            nextKey++;
            var enqueueInMemory = Count < maxItemsInMemory;
            if (enqueueInMemory)
            {
                inMemoryItems.Enqueue(Task.FromResult(item));
            }
            if (!enqueueInMemory || persistAllItems)
            {
                persistenceFirstKey = nextKey;
                persistence.Persist(nextKey, item);
            }
            Count++;
            queueSemaphore.Release();
        }

        /// <summary>
        /// Removes the top item and returns it
        /// </summary>
        /// <returns>The dequeued item</returns>
        public async Task<T> DequeueAsync()
        {
            queueSemaphore.Wait();
            try
            {
                if (Count-- == 0)
                {
                    throw new InvalidOperationException("Cannot dequeue from an empty queue");
                }
                uint keyToRemove = firstKey++;
                T item = await inMemoryItems.Dequeue();
                if (persistenceFirstKey == keyToRemove)
                {
                    persistence.Remove(keyToRemove);
                    persistenceFirstKey++;
                }
                if (inMemoryItems.Count < Count)
                {
                    uint keyToLoad = firstKey + (uint)inMemoryItems.Count;
                    Task<T> loadTask = persistence.LoadAsync(keyToLoad);
                    loadTask.ConfigureAwait(false);
                    inMemoryItems.Enqueue(loadTask);
                }
                return item;
            }
            finally
            {
                queueSemaphore.Release();
            }
        }

        /// <summary>
        /// Returns the top item without removing it
        /// </summary>
        /// <returns>The peeked item</returns>
        public Task<T> PeekAsync()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Cannot peek an empty queue");
            }
            return inMemoryItems.Peek();
        }

        /// <summary>
        /// Loads all items from persistence (only use this if deferring the load)
        /// </summary>
        public void Load()
        {
            if (!isLoaded)
            {
                isLoaded = true;
                foreach (T loadedItem in persistence.Load())
                {
                    Enqueue(loadedItem);
                }
            }
        }

        /// <summary>
        /// Clears all items from this structure and persistence
        /// </summary>
        public void Clear()
        {
            inMemoryItems.Clear();
            persistence.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> GetItems()
        {
            foreach (Task<T> item in inMemoryItems)
            {
                yield return item.GetAwaiter().GetResult();
            }
            for (uint key = (uint)inMemoryItems.Count + 1; key < Count; key++)
            {
                yield return persistence.Load(key);
            }
        }
    }
}
