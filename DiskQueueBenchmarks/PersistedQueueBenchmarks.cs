using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;

namespace PersistedQueueBenchmarks
{
    // TODO: Split out benchmark definitions into separate class files
    [MemoryDiagnoser]
    public class PersistedQueueBenchmarks
    {
        [Params(1000, 100000)]
        public int totalItems;

        [Params(1000, 10000)]
        public int itemsToKeepInMemory;

        [Params(false, true)]
        public bool useLargeData;

        [Benchmark]
        public async Task PersistentQueueInMemoryPersistence()
        {
            if (useLargeData)
            {
                await InMemoryLargeData();
            }
            else
            {
                await InMemoryInt();
            }
        }

        [Benchmark]
        public void NormalQueue()
        {
            if (useLargeData)
            {
                NormalQueueLargeData();
            }
            else
            {
                NormalQueueInt();
            }
        }

        private async Task InMemoryLargeData()
        {
            IPersistence<LargeData> persistence = new InMemoryPersistence<LargeData>();
            PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
            PersistedQueue<LargeData> queue = new PersistedQueue<LargeData>(persistence, config);
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(new LargeData());
            }
            for (int i = 0; i < totalItems; i++)
            {
                await queue.DequeueAsync();
            }
        }

        private async Task InMemoryInt()
        {
            IPersistence<int> persistence = new InMemoryPersistence<int>();
            PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
            PersistedQueue<int> queue = new PersistedQueue<int>(persistence, config);
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(i);
            }
            for (int i = 0; i < totalItems; i++)
            {
                await queue.DequeueAsync();
            }
        }

        private void NormalQueueLargeData()
        {
            Queue<LargeData> queue = new Queue<LargeData>();
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(new LargeData());
            }
            for (int i = 0; i < totalItems; i++)
            {
                queue.Dequeue();
            }
        }

        private void NormalQueueInt()
        {
            Queue<int> queue = new Queue<int>();
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(i);
            }
            for (int i = 0; i < totalItems; i++)
            {
                queue.Dequeue();
            }
        }
    }
}
