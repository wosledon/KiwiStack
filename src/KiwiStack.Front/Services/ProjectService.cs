using KiwiStack.Front.Models;

namespace KiwiStack.Front.Services;

public class ProjectService
{
    private readonly List<ProjectDto> _projects =
    [
        new() { Id = Guid.NewGuid(), Name = "数据仓库", Description = "企业数据集成平台", CreatedAt = DateTime.UtcNow.AddDays(-20) },
        new() { Id = Guid.NewGuid(), Name = "营销分析", Description = "市场营销数据分析体系", CreatedAt = DateTime.UtcNow.AddDays(-10) },
        new() { Id = Guid.NewGuid(), Name = "日志采集", Description = "采集与清洗系统日志", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new() { Id = Guid.NewGuid(), Name = "BI 报表", Description = "自助式 BI 报表", CreatedAt = DateTime.UtcNow.AddDays(-1) },
    ];

    public Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(string? keyword = null, CancellationToken ct = default)
    {
        IEnumerable<ProjectDto> query = _projects.OrderByDescending(p => p.CreatedAt);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                   || (p.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
        }
        return Task.FromResult<IReadOnlyList<ProjectDto>>(query.ToList());
    }
}
