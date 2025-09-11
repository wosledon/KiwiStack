using KiwiStack.Shared.Entities;

namespace KiwiStack.Api.Services.SQL;

public class StarRocksRefService : MysqlRefService
{
    public override string DbName => "StarRocks";

    public StarRocksRefService(DbConnector databaseConnection) : base(databaseConnection)
    {
    }

    public override Task<SqlTable> GetTableSchemaAsync(string tableName)
    {
        return base.GetTableSchemaAsync(tableName);
    }
}