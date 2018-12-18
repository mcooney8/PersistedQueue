using System;
using System.Collections;
using System.Collections.Generic;

namespace PersistedQueue
{
    internal class FixedArrayStack<T> : IEnumerable<T>
    {
        private const int DefaultStartingSize = 1024;

        private readonly int capacity;
        private T[] items;
        private int headIndex;

        public FixedArrayStack(int capacity)
        {
            this.capacity = capacity;
            int startingSize = Math.Min(DefaultStartingSize, capacity);
            items = new T[startingSize];
        }

        public int Count { get; private set; }

        public void Push(T item)
        {
            if (Count == capacity)
            {
                throw new InvalidOperationException("Cannot push any more items, stack is full");
            }
            if (Count == items.Length)
            {
                ExpandArray();
            }
            int index = (headIndex + Count) % capacity;
            items[index] = item;
            Count++;
        }

        public T Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Cannot pop an empty stack");
            }
            var itemToPop = items[headIndex];
            Count--;
            if (Count == 0)
            {
                headIndex = 0;
            }
            else
            {
                headIndex = (headIndex + 1) % capacity;
            }
            return itemToPop;
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
    }
}
