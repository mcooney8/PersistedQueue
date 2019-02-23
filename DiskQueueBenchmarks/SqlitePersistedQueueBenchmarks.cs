using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;
using PersistedQueue.Sqlite;
using System.Threading.Tasks;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class SqlitePersistedQueueBenchmarks
    {
        private const string PersistenceFilename = @"C:\Test\persistence.db";

        [Params(1000)]
        public int totalItems;

        [Params(1000)]
        public int itemsToKeepInMemory;

        [Params(false, true)]
        public bool useLargeData;

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

        private async Task Int()
        {
            IPersistence<int> persistence = new SqlitePersistence<int>(PersistenceFilename);
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

            persistence?.Clear();
            persistence?.Dispose();
        }

        private async Task LargeData()
        {
            IPersistence<LargeData> persistence = new SqlitePersistence<LargeData>(PersistenceFilename);
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

            persistence?.Clear();
            persistence?.Dispose();
        }
    }
}
