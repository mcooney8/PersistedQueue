using System;
using System.Collections.Generic;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class SelectStatement : IDisposable
    {
        private static readonly ParameterConverter<DatabaseItem> DatabaseItemypeMap =
            ParameterConverter.Builder<DatabaseItem>().Compile();

        private readonly Statement<DatabaseItem> statement;

        public SelectStatement(string tableName, Connection connection)
        {
            string Sql = $@"
                select {nameof(DatabaseItem.Key)}, {nameof(DatabaseItem.SerializedItem)}
                from {tableName}
                where {nameof(DatabaseItem.Key)} = @{nameof(DatabaseItem.Key)}";
            statement = connection.CompileStatement<DatabaseItem>(Sql);
        }

        public DatabaseItem Execute(uint key)
        {
            DatabaseItem item = new DatabaseItem
            {
                Key = key
            };
            statement.Bind(item);
            statement.Execute();
            return item;
        }

        public IEnumerable<DatabaseItem> Execute()
        {
            throw new NotImplementedException();
        }

        public void Dispose() => statement.Dispose();
    }
}