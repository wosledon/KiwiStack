using System.Data;
using Dapper;
using KiwiStack.Shared.Entities;

namespace KiwiStack.Api.Services.SQL;

public class MysqlRefService : SqlRefServiceBase
{
    public override string DbName => "MySQL";

    private readonly DbConnector _databaseConnection;

    public MysqlRefService(DbConnector databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public IDbConnection GetConnection()
    {
        var connection = new MySql.Data.MySqlClient.MySqlConnection(_databaseConnection.ConnectionString);
        connection.Open();
        return connection;
    }

    public override async Task CreateTableAsync(SqlTable table)
    {
        using var connection = GetConnection();

        var sql = table.ToSqlString("mysql");

        await connection.ExecuteAsync(sql);
    }

    public override Task<List<SqlField>> GetAllColumnNamesAsync(string tableName)
    {
        using var connection = GetConnection();

        var sql = $"SHOW FULL COLUMNS FROM `{tableName}`;";

        var columns = connection.Query(sql).ToList();

        var fields = new List<SqlField>();

        foreach (var col in columns)
        {
            var field = new SqlField
            {
                Name = col.Field,
                Type = col.Type,
                IsNullable = col.Null == "YES",
                IsPrimaryKey = col.Key == "PRI",
                DefaultValue = col.Default,
                Comment = col.Comment
            };
            fields.Add(field);
        }

        return Task.FromResult(fields);
    }

    public override Task<string[]> GetAllDatabaseNamesAsync()
    {
        using var connection = GetConnection();

        var sql = "SHOW DATABASES;";

        var databases = connection.Query<string>(sql).ToArray();

        return Task.FromResult(databases);
    }

    public override Task<string[]> GetAllTableNamesAsync(string? database = null)
    {
        using var connection = GetConnection();

        var sql = "SHOW TABLES;";
        if (database != null)
        {
            sql = $"SHOW TABLES FROM `{database}`;";
        }

        var tables = connection.Query<string>(sql).ToArray();

        return Task.FromResult(tables);
    }

    public override async Task<SqlTable> GetTableSchemaAsync(string tableName)
    {
        using var connection = GetConnection();

        var sql = $"SHOW CREATE TABLE `{tableName}`;";

        var result = await connection.QueryFirstAsync(sql);

        if (result == null)
        {
            throw new Exception($"Table '{tableName}' not found.");
        }

        var createTableSql = result["Create Table"].ToString();

        return ParseCreateTableSql(createTableSql);
    }

    private SqlTable ParseCreateTableSql(string createTableSql)
    {
        var lines = createTableSql.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var table = new SqlTable();

        // 第一行: CREATE TABLE `table_name` (
        var firstLine = lines[0].Trim();
        var tableNameStart = firstLine.IndexOf('`') + 1;
        var tableNameEnd = firstLine.IndexOf('`', tableNameStart);
        table.Name = firstLine.Substring(tableNameStart, tableNameEnd - tableNameStart);

        // 中间行: 字段定义
        for (int i = 1; i < lines.Length - 1; i++)
        {
            var line = lines[i].Trim().TrimEnd(',');

            if (line.StartsWith("PRIMARY KEY"))
            {
                // 处理主键
                var pkStart = line.IndexOf('(') + 1;
                var pkEnd = line.IndexOf(')', pkStart);
                var pkFields = line.Substring(pkStart, pkEnd - pkStart).Replace("`", "").Split(',');

                foreach (var pkField in pkFields)
                {
                    var field = table.Fields.FirstOrDefault(f => f.Name == pkField.Trim());
                    if (field != null)
                    {
                        field.IsPrimaryKey = true;
                    }
                }
            }
            else if (line.StartsWith("KEY") || line.StartsWith("UNIQUE KEY") || line.StartsWith("CONSTRAINT"))
            {
                // 忽略索引和约束定义
                continue;
            }
            else
            {
                // 处理字段定义
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var field = new SqlField
                {
                    Name = parts[0].Trim('`'),
                    Type = parts[1],
                    IsNullable = !line.Contains("NOT NULL"),
                    DefaultValue = ExtractDefaultValue(line),
                    Comment = ExtractComment(line)
                };
                table.Fields.Add(field);
            }
        }

        // 最后一行: ) ENGINE=... COMMENT='table_comment';
        var lastLine = lines[^1].Trim().TrimEnd(';');
        table.Comment = ExtractTableComment(lastLine);

        return table;
    }

    private string ExtractDefaultValue(string line)
    {
        var defaultIndex = line.IndexOf("DEFAULT", StringComparison.OrdinalIgnoreCase);
        if (defaultIndex >= 0)
        {
            var afterDefault = line.Substring(defaultIndex + 7).Trim();
            var endIndex = afterDefault.IndexOf(' ');
            if (endIndex >= 0)
            {
                return afterDefault.Substring(0, endIndex).Trim('\'', '"');
            }
            return afterDefault.Trim('\'', '"');
        }
        return string.Empty;
    }

    private string ExtractComment(string line)
    {
        var commentIndex = line.IndexOf("COMMENT", StringComparison.OrdinalIgnoreCase);
        if (commentIndex >= 0)
        {
            var afterComment = line.Substring(commentIndex + 7).Trim();
            var endIndex = afterComment.IndexOf(' ');
            if (endIndex >= 0)
            {
                return afterComment.Substring(0, endIndex).Trim('\'', '"');
            }
            return afterComment.Trim('\'', '"');
        }
        return string.Empty;
    }

    private string ExtractTableComment(string line)
    {
        var commentIndex = line.IndexOf("COMMENT", StringComparison.OrdinalIgnoreCase);
        if (commentIndex >= 0)
        {
            var afterComment = line.Substring(commentIndex + 7).Trim().TrimEnd(';');
            return afterComment.Trim('\'', '"');
        }
        return string.Empty;
    }

    public override Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            connection.Open();
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
