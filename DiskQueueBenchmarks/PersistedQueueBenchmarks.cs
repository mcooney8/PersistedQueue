using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;

namespace PersistedQueueBenchmarks
{
    [MemoryDiagnoser]
    public class PersistedQueueBenchmarks
    {
        [Params(10000)]
        public int itemsToEnqueue;

        [Params(10)]
        public int itemsToKeepInMemory;

        [Benchmark]
        public void DiskQueueEnqueue()
        {
            IPersistence<LargeData> persistence = new FlatFilePersistence<LargeData>("/Users/Michael/Projects/DiskQueue/persistence");
            PersistedQueue<LargeData> queue = new PersistedQueue<LargeData>(persistence, itemsToKeepInMemory);
            for (int i = 0; i < itemsToEnqueue; i++)
            {
                queue.Enqueue(new LargeData());
            }
            queue.Clear();
            persistence.Dispose();
        }

        [Benchmark]
        public void NormalQueueDiskEnqueue()
        {
            Queue<LargeData> queue = new Queue<LargeData>();
            for (int i = 0; i < itemsToEnqueue; i++)
            {
                queue.Enqueue(new LargeData());
            }
            queue.Clear();
        }

        [Serializable]
        private class Data
        {
            public Data()
            {
                Random random = new Random();
                DoubleValue = random.NextDouble();
                byte[] buffer = new byte[100];
                random.NextBytes(buffer);
                StringValue = Encoding.ASCII.GetString(buffer);
                BoolValue = random.Next() % 2 == 0;
            }
            public double DoubleValue { get; }
            public string StringValue { get; }
            public bool BoolValue { get; }
        }

        [Serializable]
        private class LargeData
        {
            public LargeData()
            {
                Random random = new Random();
                DoubleValue = random.NextDouble();
                byte[] buffer = new byte[1024];
                random.NextBytes(buffer);
                StringValue1 = Encoding.ASCII.GetString(buffer);
                random.NextBytes(buffer);
                StringValue2 = Encoding.ASCII.GetString(buffer);
                random.NextBytes(buffer);
                StringValue3 = Encoding.ASCII.GetString(buffer);
                BoolValue = random.Next() % 2 == 0;
            }
            public double DoubleValue { get; }
            public string StringValue1 { get; }
            public string StringValue2 { get; }
            public string StringValue3 { get; }
            public bool BoolValue { get; }
        }
    }
}
