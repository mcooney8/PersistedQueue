using System;
using PersistedQueue;
using PersistedQueue.Persistence;
using Xunit;

namespace PersistedQueueTests
{
    public class PersistedQueueTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1024)]
        [InlineData(1025)]
        [InlineData(2048)]
        public void Enqueue(int numberOfItems)
        {
            // Arrange
            PersistedQueue<int> queue = CreatePersistedQueue<int>();

            // Act
            for (int i = 0; i < numberOfItems; i++)
            {
                queue.Enqueue(i);
            }

            // Assert
            Assert.Equal(0, queue.Peek());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1024)]
        [InlineData(1025)]
        [InlineData(2048)]
        public void Dequeue(int numberOfItems)
        {
            // Arrange
            PersistedQueue<int> queue = CreatePersistedQueue<int>();
            for (int i = 0; i < numberOfItems; i++)
            {
                queue.Enqueue(i);
            }

            // Act / Assert
            for (int i = 0; i < numberOfItems; i++)
            {
                Assert.Equal(i, queue.Dequeue());
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1024)]
        [InlineData(1025)]
        [InlineData(2048)]
        public void Count(int numberOfItems)
        {
            // Arrange
            PersistedQueue<int> queue = CreatePersistedQueue<int>();
            for (int i = 0; i < numberOfItems; i++)
            {
                queue.Enqueue(i);
            }

            // Act / Assert
            for (int i = 0; i < numberOfItems; i++)
            {
                Assert.Equal(numberOfItems - i, queue.Count);
                queue.Dequeue();
            }
            Assert.Empty(queue);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1024)]
        [InlineData(1025)]
        [InlineData(2048)]
        public void GetEnumerator(int numberOfItems)
        {
            // Arrange
            PersistedQueue<int> queue = CreatePersistedQueue<int>();
            for (int i = 0; i < numberOfItems; i++)
            {
                queue.Enqueue(i);
            }

            // Act / Assert
            int expected = 0;
            foreach (int item in queue)
            {
                Assert.Equal(expected, item);
                expected++;
            }
        }

        private PersistedQueue<T> CreatePersistedQueue<T>(int inMemoryCapacity = 1024)
        {
            IPersistence<T> persistence = new InMemoryPersistence<T>();
            return  new PersistedQueue<T>(persistence, inMemoryCapacity);
        }
    }
}
