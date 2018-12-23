using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersistedQueue.Persistence;

namespace PersistedQueue
{
    /// <summary>
    /// Persisted queue.
    /// </summary>
    public class PersistedQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly IPersistence<T> persistence;
        private readonly int maxItemsInMemory;
        private readonly FixedArrayStack<Task<T>> inMemoryItems;
        private readonly object queueLock = new object();

        private uint nextKey;
        private uint firstKey = 1;

        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PersistedQueue.PersistedQueue`1"/> class.
        /// </summary>
        /// <param name="persistence">Persistence.</param>
        /// <param name="maxItemsInMemory">Max items in memory.</param>
        /// <param name="deferLoad">If set to <c>true</c> defer load.</param>
        public PersistedQueue(IPersistence<T> persistence, int maxItemsInMemory, bool deferLoad = false)
        {
            if (maxItemsInMemory < 1)
            {
                throw new ArgumentException("Must be greater than 0", nameof(maxItemsInMemory));
            }
            this.maxItemsInMemory = maxItemsInMemory;
            this.inMemoryItems = new FixedArrayStack<Task<T>>(maxItemsInMemory);
            this.persistence = persistence;
            if (!deferLoad)
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
            lock (queueLock)
            {
                nextKey++;
                persistence.Persist(nextKey, item); // TODO: Make this async?
                if (Count < maxItemsInMemory)
                {
                    inMemoryItems.Push(Task.FromResult(item));
                }
                Count++;
            }
        }

        /// <summary>
        /// Removes the top item and returns it
        /// </summary>
        /// <returns>The dequeued item</returns>
        public Task<T> DequeueAsync()
        {
            lock (queueLock)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Cannot dequeue from an empty queue");
                }
                Count--;
                uint keyToRemove = firstKey++;
                Task<T> dequeueTask = inMemoryItems.Pop();
                dequeueTask.ContinueWith(task => persistence.Remove(keyToRemove));
                if (inMemoryItems.Count < Count)
                {
                    uint keyToLoad = firstKey + (uint)inMemoryItems.Count;
                    Task<T> loadTask = persistence.LoadAsync(keyToLoad);
                    loadTask.ConfigureAwait(false);
                    inMemoryItems.Push(loadTask);
                }
                return dequeueTask;
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
            lock (queueLock)
            {
                if (!isLoaded)
                {
                    foreach (T loadedItem in persistence.Load())
                    {
                        Enqueue(loadedItem);
                    }
                    isLoaded = true;
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
