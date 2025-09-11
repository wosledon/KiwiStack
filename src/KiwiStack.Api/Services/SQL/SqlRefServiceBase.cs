using System.Data;
using System.Threading.Tasks;

namespace KiwiStack.Api.Services.SQL;

// SQL 封装基类
public abstract class SqlRefServiceBase
{
    public abstract string DbName { get; }

    public bool IsMe(string name) => string.Equals(name, DbName, StringComparison.OrdinalIgnoreCase);

    // 获取表结构
    public abstract Task<SqlTable> GetTableSchemaAsync(string tableName);

    // 获取所有表名
    public abstract Task<string[]> GetAllTableNamesAsync(string? database = null);

    // 获取所有数据库名
    public abstract Task<string[]> GetAllDatabaseNamesAsync();

    // 获取表的所有列名
    public abstract Task<List<SqlField>> GetAllColumnNamesAsync(string tableName);

    // 测试数据库连接
    public abstract Task<bool> TestConnectionAsync(string connectionString);

    // 创建表
    public abstract Task CreateTableAsync(SqlTable table);
}


public class SqlField
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string DefaultValue { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;


    public override string ToString()
    {
        return $"{Name} {Type} {(IsNullable ? "NULL" : "NOT NULL")} {(IsPrimaryKey ? "PRIMARY KEY" : "")} {(string.IsNullOrWhiteSpace(DefaultValue) ? "" : $"DEFAULT {DefaultValue}")} {(string.IsNullOrWhiteSpace(Comment) ? "" : $"COMMENT '{Comment}'")}";
    }
}

public class SqlTable
{
    public string Name { get; set; } = string.Empty;
    public List<SqlField> Fields { get; set; } = new List<SqlField>();

    public string Comment { get; set; } = string.Empty;

    public override string ToString()
    {
        var fieldsStr = string.Join(",\n  ", Fields.Select(f => f.ToString()));
        return $"CREATE TABLE {Name} (\n  {fieldsStr}\n) {(string.IsNullOrWhiteSpace(Comment) ? "" : $"COMMENT='{Comment}'")};";
    }

    public string ToSqlString(string database)
    {
        if (database.Equals("mysql", StringComparison.OrdinalIgnoreCase))
        {
            return ToString();
        }

        if (database.Equals("starRocks", StringComparison.OrdinalIgnoreCase))
        {
            return $"CREATE TABLE {Name} (\n  {string.Join(",\n  ", Fields.Select(f => $"{f.Name} {f.Type} {(f.IsNullable ? "NULL" : "NOT NULL")}"))}\n) ENGINE=OLAP\nPRIMARY KEY({string.Join(", ", Fields.Where(f => f.IsPrimaryKey).Select(f => f.Name))})\nDISTRIBUTED BY HASH({string.Join(", ", Fields.Where(f => f.IsPrimaryKey).Select(f => f.Name))}) BUCKETS 10\nPROPERTIES (\n\"replication_allocation\" = \"tag.location.default: 3\",\n\"in_memory\" = \"false\",\n\"storage_format\" = \"V2\",\n\"light_schema_change\" = \"true\",\n\"disable_auto_compaction\" = \"false\",\n\"enable_single_replica_compaction\" = \"false\"\n);";
        }

        throw new NotSupportedException($"不支持的数据库类型: {database}");
    }
}
