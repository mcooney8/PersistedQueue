using PersistedQueue.Sqlite;
using Sqlite.Fast;
using System;

namespace LogAnalyzer.PersistedQueue.Sqlite
{
    internal class Statements : IDisposable
    {
        public readonly DeleteStatement DeleteStatement;
        public readonly InsertStatement InsertStatement;
        public readonly SelectStatement SelectStatement;

        public Statements(string tableName, Connection connection)
        {
            try
            {
                DeleteStatement = new DeleteStatement(tableName, connection);
                InsertStatement = new InsertStatement(tableName, connection);
                SelectStatement = new SelectStatement(tableName, connection);
            }
            catch
            {
                DeleteStatement?.Dispose();
                InsertStatement?.Dispose();
                SelectStatement?.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            DeleteStatement.Dispose();
            InsertStatement.Dispose();
            SelectStatement.Dispose();
        }
    }
}
