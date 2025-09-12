using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Entities;

// SubProject Tree
public class SubProject : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Related Project Id
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Related Project
    /// </summary>
    public virtual Project? Project { get; set; }


    public Guid? ParentId { get; set; }
    public virtual SubProject? Parent { get; set; }

    // 版本号（语义化版本或自由格式）
    public string Version { get; set; } = "0.0.0";

    // 是否为当前活跃版本
    public bool IsActive { get; set; } = true;
}
