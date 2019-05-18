using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace PersistedQueueBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Debug().GetAwaiter().GetResult();
#else
            Release();
#endif
        }

        private static void Release()
        {
            BenchmarkRunner.Run<NosqlPersistedQueueBenchmarks>();
            //BenchmarkRunner.Run<SqlitePersistedQueueBenchmarks>();
            //BenchmarkRunner.Run<QueueBenchmarks>();
            //BenchmarkRunner.Run<InMemoryPersistedQueueBenchmarks>();
        }

        private static async Task Debug()
        {
            var benchmark = new SqlitePersistedQueueBenchmarks();
            benchmark.useLargeData = false;
            benchmark.totalItems = 10000;
            benchmark.itemsToKeepInMemory = 1;
            benchmark.IterationSetup();
            await benchmark.PersistentQueueSqliteFilePersistence();
            benchmark.IterationCleanup();
        }
    }
}
