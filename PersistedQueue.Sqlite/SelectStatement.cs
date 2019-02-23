using System;
using System.Collections.Generic;
using Sqlite.Fast;

namespace PersistedQueue.Sqlite
{
    internal class SelectStatement : IDisposable
    {
        private readonly Statement<DatabaseItem, uint> selectByIdStatement;
        private static readonly ResultConverter<DatabaseItem> resultConverter = ResultConverter.Builder<DatabaseItem>()
            .With(dbItem => dbItem.SerializedItem, (ReadOnlySpan<byte> span) => span.ToArray())
            .Compile();
        private static readonly ParameterConverter<uint> parameterConverter =
            ParameterConverter.ScalarBuilder<uint>(true)
            .Compile();

        private readonly Statement<bool, uint> existsStatement;

        public SelectStatement(string tableName, Connection connection)
        {
            string selectByIdSql = $@"select {nameof(DatabaseItem.Key)}, {nameof(DatabaseItem.SerializedItem)}
                            from {tableName}
                            where {nameof(DatabaseItem.Key)} = @key";
            selectByIdStatement = connection.CompileStatement(selectByIdSql, resultConverter, parameterConverter);

            string existsSql = $@"select exists (select 1 from {tableName} where {nameof(DatabaseItem.Key)} = @key)";
            existsStatement = connection.CompileStatement<bool, uint>(existsSql);
        }

        public DatabaseItem Execute(uint key)
        {
            DatabaseItem item = new DatabaseItem();
            selectByIdStatement.Bind(key).Execute(ref item);
            return item;
        }

        public bool Exists(uint key)
        {
            bool exists = false;
            existsStatement.Bind(key).Execute(ref exists);
            return exists;
        }

        public IEnumerable<DatabaseItem> Execute()
        {
            throw new NotImplementedException();
        }

        public void Dispose() => selectByIdStatement.Dispose();
    }
}