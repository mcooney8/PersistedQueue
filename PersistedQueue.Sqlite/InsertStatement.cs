using System;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class InsertStatement : IDisposable
    {
        private static readonly ParameterConverter<DatabaseItem> TypeMap =
            ParameterConverter.Builder<DatabaseItem>()
            .With(dbItem => dbItem.SerializedItem, (byte[] a) => a.AsSpan())
            .Compile();

        private readonly Statement<DatabaseItem> statement;

        public InsertStatement(string tableName, Connection connection)
        {
            string Sql = $@"
                insert into {tableName} ({nameof(DatabaseItem.Key)}, {nameof(DatabaseItem.SerializedItem)})
                values (@{nameof(DatabaseItem.Key)}, @{nameof(DatabaseItem.SerializedItem)})";

            statement = connection.CompileStatement(Sql, TypeMap);
        }

        public void Execute(DatabaseItem item)
        {
            statement.Bind(item);
            statement.Execute();
        }

        public void Dispose() => statement.Dispose();
    }
}
