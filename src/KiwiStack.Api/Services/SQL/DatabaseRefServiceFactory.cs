using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Entities;

namespace KiwiStack.Api.Services.SQL;

public class DatabaseRefServiceFactory : ISingletonService
{
    public SqlRefServiceBase Create(DbConnector databaseConnection)
    {
        return databaseConnection.DbType switch
        {
            Shared.DbTypeEnum.MySql => new MysqlRefService(databaseConnection),
            Shared.DbTypeEnum.StarRocks => new StarRocksRefService(databaseConnection),
            _ => throw new NotSupportedException($"不支持的数据库类型: {databaseConnection.DbType}"),
        };
    }
}
