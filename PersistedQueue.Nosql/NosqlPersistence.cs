using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DBreeze;
using DBreeze.Transactions;
using Newtonsoft.Json;
using PersistedQueue.Persistence;

namespace PersistedQueue.Nosql
{
    public class NosqlPersistence<T> : IPersistence<T>
    {
        private const string TableName = "Items";
        private const int MaxUncommitted = 1024;

        private readonly DBreezeEngine db;
        private readonly string path;
        private readonly object transactionLock = new object();
        private readonly TimeSpan MaxUncommittedTime = TimeSpan.FromSeconds(20);
        private readonly bool isObject;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private Transaction transaction;
        private int uncommittedCount;
        private Timer commitTimer;

        public NosqlPersistence(string path)
        {
            this.path = path;
            isObject = !typeof(T).IsValueType;
            var configuration = new DBreezeConfiguration
            {
                DBreezeDataFolderName = path
            };
            db = new DBreezeEngine(configuration);
            transaction = db.GetTransaction();
            commitTimer = new Timer(CommitTimerFired, null, MaxUncommittedTime, MaxUncommittedTime);
        }

        public void Clear()
        {
            Directory.Delete(path, true);
        }

        public void Dispose()
        {
            Commit();
            db.Dispose();
        }

        public IEnumerable<T> Load()
        {
            if (isObject)
            {
                lock (transactionLock)
                {
                    var rows = transaction.SelectForward<uint, string>(TableName);
                    return rows.Select(row => JsonConvert.DeserializeObject<T>(row.Value, serializerSettings));
                }
            }
            lock (transactionLock)
            {
                var rows = transaction.SelectForward<uint, T>(TableName);
                return rows.Select(row => row.Value);
            }
        }

        public T Load(uint key)
        {
            if (isObject)
            {
                lock (transactionLock)
                {
                    Commit();
                    var serialized = transaction.Select<uint, string>(TableName, key).Value;
                    return JsonConvert.DeserializeObject<T>(serialized, serializerSettings);
                }
            }
            lock (transactionLock)
            {
                return transaction.Select<uint, T>(TableName, key).Value;
            }
        }

        public Task<T> LoadAsync(uint key)
        {
            return Task.Run(() => Load(key));
        }

        public void Persist(uint key, T item)
        {
            if (isObject)
            {
                lock (transactionLock)
                {
                    transaction.Insert(TableName, key,
                        JsonConvert.SerializeObject(item, serializerSettings));
                    OnWrite();
                }
            }
            else
            {
                lock (transactionLock)
                {
                    transaction.Insert(TableName, key, item);
                    OnWrite();
                }
            }
        }

        public void Remove(uint key)
        {
            lock (transactionLock)
            {
                transaction.RemoveKey(TableName, key);
                OnWrite();
            }
        }

        private void OnWrite()
        {
            if (++uncommittedCount >= MaxUncommitted)
            {
                Commit();
            }
            else if (commitTimer == null)
            {
                commitTimer = new Timer(CommitTimerFired, null, MaxUncommittedTime, MaxUncommittedTime);
            }
            else
            {
                commitTimer.Change(MaxUncommittedTime, MaxUncommittedTime);
            }
        }

        private void CommitTimerFired(object state) => Commit();

        private void Commit()
        {
            if (uncommittedCount > 0)
            {
                transaction.Commit();
                uncommittedCount = 0;
                commitTimer?.Dispose();
                commitTimer = null;
            }
        }
    }
}
