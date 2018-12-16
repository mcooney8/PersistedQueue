using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class PersistedQueueBenchmarks
    {
        [Params(100000)]
        public int itemsToEnqueue;

        [Params(1000)]
        public int itemsToKeepInMemory;

        [Benchmark]
        public void DiskQueueEnqueue()
        {
            IPersistence<int> persistence = new InMemoryPersistence<int>();
            PersistedQueue<int> queue = new PersistedQueue<int>(persistence, itemsToKeepInMemory);
            for (int i = 0; i < itemsToEnqueue; i++)
            {
                queue.Enqueue(i);
            }
        }

        [Benchmark]
        public void NormalQueueDiskEnqueue()
        {
            Queue<int> queue = new Queue<int>();
            for (int i = 0; i < itemsToEnqueue; i++)
            {
                queue.Enqueue(i);
            }
        }
    }
}
