using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogAnalyzer.PersistedQueue.Sqlite;
using Newtonsoft.Json;
using PersistedQueue.Persistence;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    public class SqlitePersistence<T> : IPersistence<T>
    {
        private const string TableName = "PersistedItem";

        private Connection connection;
        private bool disposed;

        private static readonly string DropTableSql = $"drop table {TableName}";
        private static readonly string CreateTableSql = $"create table if not exists {TableName} " +
        	$"({nameof(DatabaseItem.Key)} INTEGER, {nameof(DatabaseItem.SerializedItem)} TEXT)";
        private const string StartTransactionSql = "begin transaction";
        private const string CommitSql = "commit";

        private DisposablePool<Statements> statementPool;
        private Statement createTableStatment;
        private Statement dropTableStatement;
        private Statement startTransactionStatement;
        private Statement commitStatement;

        private bool isTransactionOpen;
        private int uncommittedCount = 0;
        private const int MaxUncomitted = 1024;

        private readonly object transactionLock = new object();

        public SqlitePersistence(string dbFilePath)
        {
            connection = new Connection(dbFilePath);
            startTransactionStatement = connection.CompileStatement(StartTransactionSql);
            commitStatement = connection.CompileStatement(CommitSql);
            createTableStatment = connection.CompileStatement(CreateTableSql);
            createTableStatment.Execute();
            statementPool = new DisposablePool<Statements>(
                factory: () => new Statements(TableName, connection), poolSize: 64);
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
            lock (transactionLock)
            {
                if (isTransactionOpen)
                {
                    commitStatement.Execute();
                    isTransactionOpen = false;
                    uncommittedCount = 0;
                }
            }
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
            StartTransactionIfNecessary();
            DatabaseItem dbItem = Convert(key, item);
            var statements = statementPool.Rent();
            statements.InsertStatement.Execute(dbItem);
            statementPool.Return(statements);
            CommitIfNecessary();
        }

        public void Remove(uint key)
        {
            StartTransactionIfNecessary();
            var statements = statementPool.Rent();
            statements.DeleteStatement.Execute(key);
            statementPool.Return(statements);
            CommitIfNecessary();
        }

        private void StartTransactionIfNecessary()
        {
            lock (transactionLock)
            {
                if (!isTransactionOpen)
                {
                    startTransactionStatement.Execute();
                    isTransactionOpen = true;
                }
            }
        }

        private void CommitIfNecessary()
        {
            lock (transactionLock)
            {
                if (++uncommittedCount > MaxUncomitted)
                {
                    commitStatement.Execute();
                    isTransactionOpen = false;
                    Interlocked.Exchange(ref uncommittedCount, 0);
                }
            }
        }

        private T Convert(DatabaseItem dbItem)
        {
            return JsonConvert.DeserializeObject<T>(dbItem.SerializedItem);
        }

        private DatabaseItem Convert(uint key, T item)
        {
            return new DatabaseItem
            {
                Key = key,
                SerializedItem = JsonConvert.SerializeObject(item)
            };
        }
    }
}
