using System;
using System.Collections;
using System.Collections.Generic;

namespace PersistedQueue
{
    internal class FixedArrayQueue<T> : IEnumerable<T>
    {
        private const int DefaultStartingSize = 512;

        private readonly int capacity;
        private T[] items;
        private int headIndex;

        public FixedArrayQueue(int capacity)
        {
            this.capacity = capacity;
            int startingSize = Math.Min(DefaultStartingSize, capacity);
            items = new T[startingSize];
        }

        public int Count { get; private set; }

        public void Enqueue(T item)
        {
            if (Count == capacity)
            {
                throw new InvalidOperationException("Cannot push any more items, queue is full");
            }
            if ((headIndex + Count) == items.Length)
            {
                ExpandArray();
            }
            int index = (headIndex + Count) % capacity;
            items[index] = item;
            Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Cannot pop an empty queue");
            }
            var itemToDequeue = items[headIndex];
            if (--Count == 0)
            {
                ResetArray();
            }
            else
            {
                headIndex = (headIndex + 1) % capacity;
            }
            return itemToDequeue;
        }

        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Cannot peek an empty stack");
            }
            return items[headIndex];
        }

        public void Clear()
        {
            headIndex = 0;
            Count = 0;
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
            for (int i = 0; i < Count; i++)
            {
                int index = (headIndex + i) % capacity;
                yield return items[index];
            }
        }

        private void ExpandArray()
        {
            int newArrayLength = Math.Min(items.Length * 2, capacity);
            if (newArrayLength != items.Length)
            {
                T[] newArray = new T[newArrayLength];
                Array.Copy(items, headIndex, newArray, 0, Count);
                items = newArray;
                headIndex = 0;
            }
        }

        private void ResetArray()
        {
            var newArrayLength = Math.Min(DefaultStartingSize, capacity);
            items = new T[newArrayLength];
            headIndex = 0;
        }
    }
}
