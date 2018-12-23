using System;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class DeleteStatement : IDisposable
    {
        private static readonly ParameterConverter<DatabaseItem> DatabaseItemypeMap =
            ParameterConverter.Builder<DatabaseItem>().Compile();

        private readonly Statement<DatabaseItem> statement;

        public DeleteStatement(string tableName, Connection connection)
        {
            string Sql = $@"
                delete from {tableName}
                where {nameof(DatabaseItem.Key)} = @{nameof(DatabaseItem.Key)}";
            statement = connection.CompileStatement<DatabaseItem>(Sql);
        }

        public void Execute(uint key)
        {
            DatabaseItem item = new DatabaseItem
            {
                Key = key
            };
            statement.Bind(item);
            statement.Execute();
        }

        public void Dispose() => statement.Dispose();
    }
}
