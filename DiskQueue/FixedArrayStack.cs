using System;
using System.Collections;
using System.Collections.Generic;

namespace PersistedQueue
{
    internal class FixedArrayStack<T> : IEnumerable<T>
    {
        private readonly int capacity;
        private T[] items;
        private int headIndex;

        public FixedArrayStack(int capacity)
        {
            this.capacity = capacity;
            items = new T[capacity]; // TODO: only allocate up to some max starting amount and then expand as needed
        }

        public int Count { get; private set; }

        public void Push(T item)
        {
            if (Count == capacity)
            {
                throw new InvalidOperationException("Cannot push any more items, stack is full");
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
    }
}
