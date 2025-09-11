using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Entities;

public class DbConnector : EntityBase
{
    public string Name { get; set; } = string.Empty;

    // 将原来的 ConnectionString 展开为结构化字段
    public DbTypeEnum DbType { get; set; } = DbTypeEnum.MySql;

    public string Host { get; set; } = string.Empty;
    public int? Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public EnvironmentEnum Environment { get; set; } = EnvironmentEnum.Development;


    // 兼容原来的 ConnectionString 字段：读取时根据字段构建，写入时解析回字段
    public string ConnectionString
    {
        get => BuildConnectionString();
        set => ParseConnectionString(value ?? string.Empty);
    }

    private string BuildConnectionString()
    {
        string joinOpts(string sep, string prefix = "") =>
            Options != null && Options.Any() ? prefix + string.Join(sep, Options.Select(kv => $"{kv.Key}={kv.Value}")) : "";

        switch (DbType)
        {
            case DbTypeEnum.SqlServer:
                {
                    var server = string.IsNullOrEmpty(Host) ? "localhost" : Host;
                    var portPart = Port.HasValue ? "," + Port.Value : "";
                    var opts = joinOpts(";", ";");
                    return $"Server={server}{portPart};Database={Database};User Id={Username};Password={Password}{opts}";
                }
            case DbTypeEnum.MySql:
            case DbTypeEnum.StarRocks:
                {
                    var portPart = Port.HasValue ? Port.Value.ToString() : "";
                    var opts = joinOpts(";", ";");
                    return $"Server={Host};Port={portPart};Database={Database};Uid={Username};Pwd={Password}{opts}";
                }
            case DbTypeEnum.PostgreSql:
                {
                    var portPart = Port.HasValue ? Port.Value.ToString() : "";
                    var opts = joinOpts(";", ";");
                    return $"Host={Host};Port={portPart};Database={Database};Username={Username};Password={Password}{opts}";
                }
            case DbTypeEnum.SQLite:
                {
                    var opts = joinOpts(";", ";");
                    return $"Data Source={Database}{opts}";
                }
            case DbTypeEnum.Oracle:
                {
                    var portPart = Port.HasValue ? Port.Value.ToString() : "";
                    var opts = joinOpts(";", ";");
                    return $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={Host})(PORT={portPart}))(CONNECT_DATA=(SERVICE_NAME={Database})));User Id={Username};Password={Password}{opts}";
                }
            case DbTypeEnum.MongoDB:
                {
                    var opts = Options != null && Options.Any() ? "?" + string.Join("&", Options.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")) : "";
                    var credential = string.IsNullOrEmpty(Username) ? "" : $"{Uri.EscapeDataString(Username)}:{Uri.EscapeDataString(Password)}@";
                    var portPart = Port.HasValue ? $":{Port.Value}" : "";
                    return $"mongodb://{credential}{Host}{portPart}/{Database}{opts}";
                }
            default:
                return string.Empty;
        }
    }

    private void ParseConnectionString(string cs)
    {
        // reset
        Host = Username = Password = Database = string.Empty;
        Port = null;
        Options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(cs)) return;

        // Mongo URI 优先检测
        if (cs.StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase) || DbType == DbTypeEnum.MongoDB)
        {
            try
            {
                var uriStr = cs.StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase) ? cs : "mongodb://" + cs;
                var uri = new Uri(uriStr);
                Host = uri.Host;
                Port = uri.IsDefaultPort ? null : uri.Port;
                if (!string.IsNullOrEmpty(uri.UserInfo))
                {
                    var parts = uri.UserInfo.Split(':');
                    Username = Uri.UnescapeDataString(parts.ElementAtOrDefault(0) ?? "");
                    Password = Uri.UnescapeDataString(parts.ElementAtOrDefault(1) ?? "");
                }
                Database = uri.AbsolutePath?.TrimStart('/') ?? string.Empty;
                var query = uri.Query?.TrimStart('?') ?? "";
                if (!string.IsNullOrEmpty(query))
                {
                    foreach (var kv in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var pair = kv.Split(new[] { '=' }, 2);
                        var k = Uri.UnescapeDataString(pair[0]);
                        var v = pair.Length > 1 ? Uri.UnescapeDataString(pair[1]) : "";
                        Options[k] = v;
                    }
                }
            }
            catch
            {
                // ignore parse errors，保持字段重置或原始值
            }
            return;
        }

        // 通用 "key=value;key2=value2" 解析
        var pairs = cs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(new[] { '=' }, 2))
            .Where(a => a.Length == 2)
            .ToDictionary(a => a[0].Trim(), a => a[1].Trim(), StringComparer.OrdinalIgnoreCase);

        string Get(params string[] keys)
        {
            foreach (var k in keys)
            {
                if (pairs.TryGetValue(k, out var v)) return v;
            }
            return string.Empty;
        }

        Host = Get("Server", "Host", "Data Source");
        var portStr = Get("Port");
        if (int.TryParse(portStr, out var p)) Port = p; else Port = null;
        Username = Get("User Id", "Uid", "Username", "User");
        Password = Get("Password", "Pwd");
        Database = Get("Database", "Initial Catalog", "Service Name", "Data Source");

        // 其余作为 Options
        var ignored = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Server","Host","Port","Database","Initial Catalog","User Id","Uid","Username","User","Password","Pwd","Data Source","Service Name"
        };
        foreach (var kv in pairs)
        {
            if (!ignored.Contains(kv.Key))
            {
                Options[kv.Key] = kv.Value;
            }
        }
    }
}
