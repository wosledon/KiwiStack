using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Entities;

// SubProject Tree
/// <summary>
/// Represents a sub-project or component which can form a tree structure under a Project.
/// </summary>
public class SubProject : EntityBase
{
    /// <summary>
    /// Name of the sub-project.
    /// Defaults to an empty string.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the sub-project.
    /// Defaults to an empty string.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Related Project Id
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Related Project
    /// </summary>
    public virtual Project? Project { get; set; }

    /// <summary>
    /// Parent SubProject Id
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Parent SubProject
    /// </summary>
    public virtual SubProject? Parent { get; set; }
}
