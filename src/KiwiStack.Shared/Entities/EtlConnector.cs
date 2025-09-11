using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Entities;

/// <summary>
/// Etl connector entity
/// </summary>
public class EtlConnector : EntityBase
{
    /// <summary>
    /// Etl name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Etl description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Source database connection (can be null for gradual migration)
    /// </summary>
    public Guid? SourceDbConnectorId { get; set; }
    /// <summary>
    /// Source database connection
    /// </summary>
    public virtual DbConnector? SourceDbConnector { get; set; }

    /// <summary>
    /// Target database connection (can be null for gradual migration)
    /// </summary>
    public Guid? TargetDbConnectorId { get; set; }
    /// <summary>
    /// Target database connection
    /// </summary>
    public virtual DbConnector? TargetDbConnector { get; set; }

    /// <summary>
    /// Related project component Id
    /// </summary>
    public Guid ProjectComponentId { get; set; }
    /// <summary>
    /// Related project component
    /// </summary>
    public virtual SubProject? ProjectComponent { get; set; }

    public List<EtlSourceTarget> SourceTargets { get; set; } = new List<EtlSourceTarget>();
}
