using System;
using BenchmarkDotNet.Running;

namespace PersistedQueueBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<PersistedQueueBenchmarks>();
            var benchmark = new PersistedQueueBenchmarks();
            benchmark.useLargeData = false;
            benchmark.totalItems = 1000;
            benchmark.itemsToKeepInMemory = 100;
            benchmark.PersistentQueueSqliteFilePersistence();   
        }
    }
}
