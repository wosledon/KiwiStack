using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Entities;

public class EtlJob : EntityBase
{
    public Guid EtlConnectorId { get; set; }
    public virtual EtlConnector? EtlConnector { get; set; }

    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }

    public JobStatusEnum Status { get; set; } = JobStatusEnum.Pending;

    public string Result { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
}
