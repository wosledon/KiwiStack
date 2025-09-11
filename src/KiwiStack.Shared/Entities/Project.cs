using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Entities;

public class Project : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Prefix { get; set; } = string.Empty;

    // 子级实体：用于用户对项目进行分组与版本划分
    public virtual ICollection<SubProject> Components { get; set; } = new List<SubProject>();
}
