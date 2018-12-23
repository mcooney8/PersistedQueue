using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PersistedQueue.Persistence;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    public class SqlitePersistence<T> : IPersistence<T>
    {
        private const string TableName = "PersistedItem";

        private readonly BinaryFormatter binaryFormatter;
        private readonly string dbFilePath;

        private Connection connection;
        private bool disposed;

        public SqlitePersistence(string dbFilePath)
        {
            this.dbFilePath = dbFilePath;
            connection = new Connection(dbFilePath);
            binaryFormatter = new BinaryFormatter();
        }

        public void Clear()
        {
            connection.Dispose();
            File.Delete(dbFilePath);
            connection = new Connection(dbFilePath);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                connection.Dispose();
                disposed = true;
            }
        }

        public IEnumerable<T> Load()
        {
            SelectStatement select = new SelectStatement(TableName, connection);
            foreach (DatabaseItem dbItem in select.Execute())
            {
                yield return Convert(dbItem);
            }
        }

        public T Load(uint key)
        {
            SelectStatement select = new SelectStatement(TableName, connection);
            DatabaseItem dbItem = select.Execute(key);
            return Convert(dbItem);
        }

        public Task<T> LoadAsync(uint key)
        {
            return Task.Run(() => Load(key));
        }

        public void Persist(uint key, T item)
        {
            DatabaseItem dbItem = Convert(key, item);
            InsertStatement insert = new InsertStatement(TableName, connection);
            insert.Execute(dbItem);
        }

        public void Remove(uint key)
        {
            DeleteStatement delete = new DeleteStatement(TableName, connection);
            delete.Execute(key);
        }

        private T Convert(DatabaseItem dbItem)
        {
            Stream serializedItemStream = new MemoryStream(dbItem.SerializedItem);
            return (T)binaryFormatter.Deserialize(serializedItemStream);
        }

        private DatabaseItem Convert(uint key, T item)
        {
            MemoryStream serialized = new MemoryStream();
            binaryFormatter.Serialize(serialized, item);
            return new DatabaseItem
            {
                Key = key,
                SerializedItem = serialized.ToArray()
            };
        }
    }
}
