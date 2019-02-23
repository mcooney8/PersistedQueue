using System;
using BenchmarkDotNet.Running;

namespace PersistedQueueBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Debug();
#else
            Release();
#endif
        }

        private static void Release()
        {
            //BenchmarkRunner.Run<SqlitePersistedQueueBenchmarks>();
            BenchmarkRunner.Run<PersistedQueueBenchmarks>();
        }

        private static void Debug()
        {
            var benchmark = new SqlitePersistedQueueBenchmarks();
            benchmark.useLargeData = false;
            benchmark.totalItems = 1000;
            benchmark.itemsToKeepInMemory = 1000;
            benchmark.PersistentQueueSqliteFilePersistence().GetAwaiter().GetResult();
        }
    }
}
