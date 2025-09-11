using System;

namespace KiwiStack.Shared.Dtos.ProjectComponent;

public class ProjectComponentCreateOrUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Version { get; set; } = "0.0.0";
    public bool IsActive { get; set; } = true;
}
