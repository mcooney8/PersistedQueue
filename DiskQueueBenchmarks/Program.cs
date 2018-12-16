using System;
using BenchmarkDotNet.Running;

namespace PersistedQueueBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PersistedQueueBenchmarks>();
            Console.ReadKey();
        }
    }
}
