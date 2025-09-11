using System;

namespace KiwiStack.Shared.Dtos.EtlConnector;

public class EtlConnectorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? SourceDatabaseConnectionId { get; set; }
    public Guid? TargetDatabaseConnectionId { get; set; }
    public Guid ProjectComponentId { get; set; }
    public string DataX { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
}
