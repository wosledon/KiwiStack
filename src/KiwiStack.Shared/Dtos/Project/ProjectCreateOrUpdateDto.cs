using System;

namespace KiwiStack.Shared.Dtos.Project;

public class ProjectCreateOrUpdateDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Prefix { get; set; } = string.Empty;
}


public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string Prefix { get; set; } = string.Empty;
}