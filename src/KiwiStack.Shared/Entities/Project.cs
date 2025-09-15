using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Entities;

/// <summary>
/// Represents a project which may contain multiple sub-projects/components.
/// </summary>
public class Project : EntityBase
{
    /// <summary>
    /// Project name.
    /// Defaults to an empty string.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the project.
    /// Defaults to an empty string.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Optional prefix used to distinguish project resources.
    /// Defaults to an empty string.
    /// </summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// Child sub-projects or components that belong to this project.
    /// Used to group related functionality or versions under the same project.
    /// </summary>
    public virtual ICollection<SubProject> Components { get; set; } = new List<SubProject>();
}
