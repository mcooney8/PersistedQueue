using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogAnalyzer.PersistedQueue.Sqlite;
using MessagePack;
using PersistedQueue.Persistence;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    public class SqlitePersistence<T> : IPersistence<T>
    {
        private const string TableName = "PersistedItem";

        private Connection connection;
        private bool disposed;

        private DisposablePool<Statements> statementPool;

        private static readonly string CreateTableSql = $"create table if not exists {TableName} ({nameof(DatabaseItem.Key)} INTEGER, {nameof(DatabaseItem.SerializedItem)} BLOB)";
        private static readonly string DropTableSql = $"drop table {TableName}";

        private Statement createTableStatment;
        private Statement dropTableStatement;

        public SqlitePersistence(string dbFilePath)
        {
            connection = new Connection(dbFilePath);
            createTableStatment = connection.CompileStatement(CreateTableSql);
            createTableStatment.Execute();
            statementPool = new DisposablePool<Statements>(factory: () => new Statements(TableName, connection), poolSize: 32);
        }

        public void Clear()
        {
            if (dropTableStatement == null)
            {
                dropTableStatement = connection.CompileStatement(DropTableSql);
            }
            dropTableStatement.Execute();
            createTableStatment.Execute();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                connection.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                disposed = true;
            }
        }

        public IEnumerable<T> Load()
        {
            return new List<T>();
        }

        public T Load(uint key)
        {
            var statements = statementPool.Rent();
            DatabaseItem dbItem = statements.SelectStatement.Execute(key);
            var result = Convert(dbItem);
            statementPool.Return(statements);
            return result;
        }

        public bool Contains(uint key)
        {
            var statements = statementPool.Rent();
            bool result = statements.SelectStatement.Exists(key);
            statementPool.Return(statements);
            return result;
        }

        public Task<T> LoadAsync(uint key)
        {
            return Task.Run(() => Load(key));
        }

        public void Persist(uint key, T item)
        {
            DatabaseItem dbItem = Convert(key, item);
            var statements = statementPool.Rent();
            statements.InsertStatement.Execute(dbItem);
            statementPool.Return(statements);
        }

        public void Remove(uint key)
        {
            var statements = statementPool.Rent();
            statements.DeleteStatement.Execute(key);
            statementPool.Return(statements);
        }

        private T Convert(DatabaseItem dbItem)
        {
            return MessagePackSerializer.Deserialize<T>(dbItem.SerializedItem);
        }

        private DatabaseItem Convert(uint key, T item)
        {
            return new DatabaseItem
            {
                Key = key,
                SerializedItem = MessagePackSerializer.Serialize(item)
            };
        }
    }
}
