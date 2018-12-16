using System;
using System.Collections;
using System.Collections.Generic;
using PersistedQueue.Persistence;

namespace PersistedQueue
{
    // TODO: summary comments for public methods and class level
    public class PersistedQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly IPersistence<T> persistence;
        private readonly int maxItemsInMemory;
        private readonly object queueLock = new object();

        private uint nextKey;
        private FixedArrayStack<T> loadedItems;
        private FixedArrayStack<uint> loadedItemsKeys; // TODO: There has to be a better way to do this
        private ArrayStack<uint> notLoadedKeys = new ArrayStack<uint>();

        // Default persistence is Sqlite
        public PersistedQueue(string filename, int maxItemsInMemory)
            : this(new SqlitePersistence<T>(filename), maxItemsInMemory)
        {
        }

        public PersistedQueue(IPersistence<T> persistence, int maxItemsInMemory)
        {
            if (maxItemsInMemory < 1)
            {
                throw new ArgumentException("Must be greater than 0", nameof(maxItemsInMemory));
            }
            this.maxItemsInMemory = maxItemsInMemory;
            this.loadedItems = new FixedArrayStack<T>(maxItemsInMemory);
            this.loadedItemsKeys = new FixedArrayStack<uint>(maxItemsInMemory);
            this.persistence = persistence;
        }

        public int Count { get; private set; }

        public void Enqueue(T item)
        {
            lock (queueLock)
            {
                nextKey++;
                persistence.Persist(nextKey, item);
                if (Count < maxItemsInMemory)
                {
                    loadedItems.Push(item);
                    loadedItemsKeys.Push(nextKey);
                }
                else
                {
                    notLoadedKeys.Push(nextKey);
                }
                Count++;
            }
        }

        public T Dequeue()
        {
            lock (queueLock)
            {
                if (loadedItems.Count == 0)
                {
                    throw new InvalidOperationException("Cannot dequeue from an empty queue");
                }
                T itemToDequeue = loadedItems.Pop();
                persistence.Remove(loadedItemsKeys.Pop());
                if (notLoadedKeys.Count > 0)
                {
                    uint keyToLoad = notLoadedKeys.Pop();
                    T newlyLoadedItem = persistence.Load(keyToLoad);
                    loadedItems.Push(newlyLoadedItem);
                    loadedItemsKeys.Push(keyToLoad);
                }
                Count--;
                return itemToDequeue;
            }
        }

        public T Peek()
        {
            if (loadedItems.Count == 0)
            {
                throw new InvalidOperationException("Cannot peek an empty queue");
            }
            return loadedItems.Peek();
        }

        public void Load()
        {
            foreach (T loadedItem in persistence.Load())
            {
                Enqueue(loadedItem);
            }
        }

        public void Clear()
        {
            loadedItems.Clear();
            notLoadedKeys.Clear();
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
            foreach (T item in loadedItems)
            {
                yield return item;
            }
            foreach (uint key in notLoadedKeys)
            {
                yield return persistence.Load(key);
            }
        }
    }
}
