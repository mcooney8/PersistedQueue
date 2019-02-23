using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;
using System.Threading.Tasks;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class InMemoryPersistedQueueBenchmarks
    {
        [Params(1000, 100000)]
        public int totalItems;

        [Params(1000)]
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
    }
}
