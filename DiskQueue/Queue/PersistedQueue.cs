using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly FixedArrayStack<T> inMemoryItems;
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
            this.inMemoryItems = new FixedArrayStack<T>(maxItemsInMemory);
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
                persistence.Persist(nextKey, item);
                if (Count < maxItemsInMemory)
                {
                    inMemoryItems.Push(item);
                }
                Count++;
            }
        }

        /// <summary>
        /// Removes the top item and returns it
        /// </summary>
        /// <returns>The dequeued item</returns>
        public T Dequeue()
        {
            lock (queueLock)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Cannot dequeue from an empty queue");
                }
                Count--;
                T itemToDequeue = inMemoryItems.Pop();
                persistence.Remove(firstKey++);
                if (inMemoryItems.Count < Count)
                {
                    LoadNextItem();
                }
                return itemToDequeue;
            }
        }

        /// <summary>
        /// Returns the top item without removing it
        /// </summary>
        /// <returns>The peeked item</returns>
        public T Peek()
        {
            if (inMemoryItems.Count == 0)
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
            foreach (T item in inMemoryItems)
            {
                yield return item;
            }
            for (uint key = (uint)inMemoryItems.Count + 1; key < Count; key++)
            {
                yield return persistence.Load(key);
            }
        }

        private void LoadNextItem()
        {
            // TODO: Use background thread for this load operation and then add
            // logic to make sure we wait for an item to be loaded before doing other operations
            uint keyToLoad = firstKey + (uint)inMemoryItems.Count;
            T newlyLoadedItem = persistence.Load(keyToLoad);
            inMemoryItems.Push(newlyLoadedItem);
        }
    }
}
