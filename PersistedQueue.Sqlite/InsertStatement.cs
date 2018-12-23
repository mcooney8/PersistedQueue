using System;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class InsertStatement : IDisposable
    {
        private static readonly ParameterConverter<DatabaseItem> DatabaseItemypeMap =
            ParameterConverter.Builder<DatabaseItem>().Compile();

        private readonly Statement<DatabaseItem> statement;

        public InsertStatement(string tableName, Connection connection)
        {
            string Sql = $@"
                insert into {tableName} ({nameof(DatabaseItem.Key)}, {nameof(DatabaseItem.SerializedItem)})
                values (@{nameof(DatabaseItem.Key)}, @{nameof(DatabaseItem.SerializedItem)})";
            statement = connection.CompileStatement<DatabaseItem>(Sql);
        }

        public void Execute(DatabaseItem item)
        {
            statement.Bind(item);
            statement.Execute();
        }

        public void Dispose() => statement.Dispose();
    }
}
