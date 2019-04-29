using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DBreeze;
using DBreeze.Transactions;
using DBreeze.Utils;
using MessagePack;
using PersistedQueue.Persistence;

namespace PersistedQueue.Nosql
{
    public class NosqlPersistence<T> : IPersistence<T>
    {
        private const string TableName = "Items";

        private readonly DBreezeEngine db;
        private readonly string path;
        private Transaction transaction;
        private bool isCommitted;
        private readonly object transactionLock = new object();

        public NosqlPersistence(string path)
        {
            this.path = path;
            var configuration = new DBreezeConfiguration
            {
                DBreezeDataFolderName = path
            };
            db = new DBreezeEngine(configuration);
            CustomSerializator.ByteArraySerializator = MessagePackSerializer.Serialize;
            CustomSerializator.ByteArrayDeSerializator =
                (byte[] serialized, Type t) => MessagePackSerializer.Deserialize<T>(serialized);
            transaction = db.GetTransaction();
        }

        public void Clear()
        {
            Directory.Delete(path, true);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IEnumerable<T> Load()
        {
            // TODO
            return new List<T>();
        }

        public T Load(uint key)
        {
            lock (transactionLock)
            {
                if (!isCommitted)
                {
                    transaction.Commit();
                    isCommitted = true;
                }
                return transaction.Select<uint, T>(TableName, key).Value;
            }
        }

        public Task<T> LoadAsync(uint key)
        {
            return Task.Run(() => Load(key));
        }

        public void Persist(uint key, T item)
        {
            lock (transactionLock)
            {
                transaction.Insert(TableName, key, item);
                isCommitted = false;
            }
        }

        public void Remove(uint key)
        {
            lock (transactionLock)
            {
                transaction.RemoveKey(TableName, key);
                isCommitted = false;
            }
        }
    }
}
