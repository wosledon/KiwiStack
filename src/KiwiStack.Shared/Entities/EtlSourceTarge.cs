using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Entities;

public class EtlSourceTarget : EntityBase
{
    public Guid EtlConnectorId { get; set; }
    public virtual EtlConnector? EtlConnector { get; set; }


    /// <summary>
    /// Source database name (logical name, not the database name in the connection string)
    /// </summary>
    public string SourceDbName { get; set; } = string.Empty;
    /// <summary>
    /// Source table name
    /// </summary>
    public string SourceTableName { get; set; } = string.Empty;

    /// <summary>
    /// Target database name (logical name, not the database name in the connection string)
    /// </summary>
    public string TargetDbName { get; set; } = string.Empty;
    /// <summary>
    /// Target table name
    /// </summary>
    public string TargetTableName { get; set; } = string.Empty;

    /// <summary>
    /// Etl type
    /// </summary>
    public EtlTypeEnum Type { get; set; }

    /// <summary>
    /// Etl script
    /// </summary>
    public string Script { get; set; } = "{}";
}
