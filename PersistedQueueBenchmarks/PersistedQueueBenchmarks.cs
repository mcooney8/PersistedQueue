using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class QueueBenchmarks
    {
        [Params(1000, 100000)]
        public int totalItems;

        [Params(1000)]
        public int itemsToKeepInMemory;

        [Params(false, true)]
        public bool useLargeData;

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
