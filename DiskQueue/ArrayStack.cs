using System;
using System.Collections;
using System.Collections.Generic;

namespace PersistedQueue
{
    internal class ArrayStack<T> : IEnumerable<T>
    {
        private const int ResizeBuffer = 128;

        private T[] items;
        private int headIndex;

        public ArrayStack() : this(1024)
        {
        }

        public ArrayStack(int defaultCapacity)
        {
            items = new T[defaultCapacity];
        }

        public int Count { get; private set; }

        public void Push(T item)
        {
            if (headIndex + Count == items.Length)
            {
                ResizeArray();
            }
            int index = headIndex + Count;
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
                headIndex++;
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
                int index = headIndex + i;
                yield return items[index];
            }
        }

        private void ResizeArray()
        {
            // TODO: Also need to add code to reduce the size of the array
            int newArrayLength = items.Length * 2;
            if (Count < (items.Length - ResizeBuffer))
            {
                newArrayLength = items.Length;
            }
            T[] newArray = new T[newArrayLength];
            Array.Copy(items, headIndex, newArray, 0, Count);
            items = newArray;
            headIndex = 0;
        }
    }
}
