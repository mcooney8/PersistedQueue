using System;
using BenchmarkDotNet.Running;

namespace PersistedQueueBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PersistedQueueBenchmarks>();
            /*
            var benchmark = new PersistedQueueBenchmarks();
            benchmark.itemsToEnqueue = 100;
            benchmark.itemsToKeepInMemory = 10;
            benchmark.enqueueOnly = false;
            benchmark.FlatFileDiskQueueEnqueue();
            Console.ReadKey();
            */
        }
    }
}
