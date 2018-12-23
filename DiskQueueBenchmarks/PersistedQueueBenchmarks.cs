using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using PersistedQueue;
using PersistedQueue.Persistence;
using PersistedQueue.Sqlite;

namespace PersistedQueueBenchmarks
{
    // TODO: Split out benchmark definitions into separate class files
    [MemoryDiagnoser]
    public class PersistedQueueBenchmarks
    {
        private const string PersistenceFilename = "persistence";

        [Params(1000)]
        public int totalItems;

        [Params(100)]
        public int itemsToKeepInMemory;

        [Params(true)]
        public bool useLargeData;

        //[GlobalSetup]
        public void GlobalSetup()
        {
            IPersistence<LargeData> persistence = new FlatFilePersistence<LargeData>(PersistenceFilename);
            persistence.Clear();
            persistence.Dispose();
            if (new FileInfo(PersistenceFilename).Length > 0)
            {
                throw new Exception("Unexpected length");
            }
        }

        [Benchmark]
        public void PersistentQueueSqliteFilePersistence()
        {
            IPersistence<LargeData> persistence = null;
            try
            {
                persistence = new SqlitePersistence<LargeData>(PersistenceFilename);
                PersistedQueue<LargeData> queue = new PersistedQueue<LargeData>(persistence, itemsToKeepInMemory);
                for (int i = 0; i < totalItems; i++)
                {
                    queue.Enqueue(new LargeData());
                }
            }
            finally
            {
                persistence?.Clear();
                persistence?.Dispose();
            }
        }

        //[Benchmark]
        public void PersistentQueueFlatFilePersistence()
        {
            if (new FileInfo(PersistenceFilename).Length > 0)
            {
                throw new Exception("Unexpected length");
            }
            if (useLargeData)
            {
                FlatFileLargeData();
            }
            else
            {
                FlatFileInt();
            }
        }

        [Benchmark]
        public void PersistentQueueInMemoryPersistence()
        {
            if (useLargeData)
            {
                InMemoryLargeData();
            }
            else
            {
                InMemoryInt();
            }
        }

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

        private void FlatFileLargeData()
        {
            IPersistence<LargeData> persistence = null;
            try
            {
                persistence = new FlatFilePersistence<LargeData>(PersistenceFilename);
                PersistedQueue<LargeData> queue = new PersistedQueue<LargeData>(persistence, itemsToKeepInMemory);
                for (int i = 0; i < totalItems; i++)
                {
                    queue.Enqueue(new LargeData());
                }
                for (int i = 0; i < totalItems; i++)
                {
                    queue.DequeueAsync();
                }
            }
            finally
            {
                persistence?.Clear();
                persistence?.Dispose();
            }
        }

        private void FlatFileInt()
        {
            IPersistence<int> persistence = null;
            try
            {
                persistence = new FlatFilePersistence<int>(PersistenceFilename);
                PersistedQueue<int> queue = new PersistedQueue<int>(persistence, itemsToKeepInMemory);
                for (int i = 0; i < totalItems; i++)
                {
                    queue.Enqueue(i);
                }
                for (int i = 0; i < totalItems; i++)
                {
                    queue.DequeueAsync();
                }
            }
            finally
            {
                persistence?.Clear();
                persistence?.Dispose();
            }
        }

        private void InMemoryLargeData()
        {
            IPersistence<LargeData> persistence = new InMemoryPersistence<LargeData>();
            PersistedQueue<LargeData> queue = new PersistedQueue<LargeData>(persistence, itemsToKeepInMemory);
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(new LargeData());
            }
            for (int i = 0; i < totalItems; i++)
            {
                queue.DequeueAsync();
            }
        }

        private void InMemoryInt()
        {
            IPersistence<int> persistence = new InMemoryPersistence<int>();
            PersistedQueue<int> queue = new PersistedQueue<int>(persistence, itemsToKeepInMemory);
            for (int i = 0; i < totalItems; i++)
            {
                queue.Enqueue(i);
            }
            for (int i = 0; i < totalItems; i++)
            {
                queue.DequeueAsync();
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
