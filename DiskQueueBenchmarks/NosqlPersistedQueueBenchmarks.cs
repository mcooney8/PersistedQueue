using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Nosql;
using PersistedQueue.Persistence;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class NosqlPersistedQueueBenchmarks
    {
        private const string PersistenceFilePath = @"/Users/Michael/Test/Persistence";

        [Params(10000)]
        public int totalItems;

        [Params(100, 10000)]
        public int itemsToKeepInMemory;

        [Params(false)]
        public bool useLargeData;

        private IPersistence<int> smallPersistence;
        private PersistedQueue<int> smallQueue;
        private IPersistence<LargeData> largePersistence;
        private PersistedQueue<LargeData> largeQueue;

        [Benchmark]
        public Task PersistentQueueSqliteFilePersistence()
        {
            if (useLargeData)
            {
                return LargeData();
            }
            else
            {
                return Int();
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            if (useLargeData)
            {
                largePersistence = new NosqlPersistence<LargeData>(PersistenceFilePath);
                PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
                largeQueue = new PersistedQueue<LargeData>(largePersistence, config);
            }
            else
            {
                smallPersistence = new NosqlPersistence<int>(PersistenceFilePath);
                PersistedQueueConfiguration config = new PersistedQueueConfiguration { MaxItemsInMemory = itemsToKeepInMemory };
                smallQueue = new PersistedQueue<int>(smallPersistence, config);
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            if (useLargeData)
            {
                largePersistence?.Clear();
                largePersistence?.Dispose();
            }
            else
            {
                smallPersistence?.Clear();
                smallPersistence?.Dispose();
            }
        }

        private async Task Int()
        {
            for (int i = 0; i < totalItems; i++)
            {
                smallQueue.Enqueue(i);
            }
            for (int i = 0; i < totalItems; i++)
            {
                await smallQueue.DequeueAsync();
            }
        }

        private async Task LargeData()
        {

            for (int i = 0; i < totalItems; i++)
            {
                largeQueue.Enqueue(new LargeData());
            }
            for (int i = 0; i < totalItems; i++)
            {
                await largeQueue.DequeueAsync();
            }
        }
    }
}
