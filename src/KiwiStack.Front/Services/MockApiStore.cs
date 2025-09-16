using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Dtos.DatabaseConnection;
using KiwiStack.Shared.Dtos.EtlConnector;
using KiwiStack.Shared.Dtos.Project;
using KiwiStack.Shared.Dtos.ProjectComponent;
using KiwiStack.Shared.Dtos.User;
using KiwiStack.Shared; // DbTypeEnum

namespace KiwiStack.Front.Services;

public class MockApiStore
{
    public List<ProjectDto> Projects { get; } = new();
    public List<DatabaseConnectionDto> DbConns { get; } = new();
    public List<ProjectComponentDto> SubProjects { get; } = new();
    public List<EtlConnectorDto> EtlConns { get; } = new();
    public List<UserDto> Users { get; } = new();

    private bool _seeded;

    public void EnsureSeed()
    {
        if (_seeded) return;
        _seeded = true;

        // Seed projects
        var p1 = new ProjectDto { Id = Guid.NewGuid(), Name = "���ݲֿ�", Description = "��ҵ���ݼ���ƽ̨", Prefix = "dw", CreatedAt = DateTime.UtcNow.AddDays(-20) };
        var p2 = new ProjectDto { Id = Guid.NewGuid(), Name = "Ӫ������", Description = "�г�Ӫ�����ݷ�����ϵ", Prefix = "mkt", CreatedAt = DateTime.UtcNow.AddDays(-10) };
        Projects.AddRange(new[] { p1, p2 });

        // Seed db connections
        DbConns.AddRange(new[]
        {
            new DatabaseConnectionDto { Id = Guid.NewGuid(), Name = "����", DbType = DbTypeEnum.MySql, Host = "localhost", Port = 3306, Username = "root", Database = "app", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new DatabaseConnectionDto { Id = Guid.NewGuid(), Name = "ODS", DbType = DbTypeEnum.StarRocks, Host = "sr-host", Port = 9030, Username = "admin", Database = "ods", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow.AddDays(-2) }
        });

        // Seed sub projects
        SubProjects.AddRange(new[]
        {
            new ProjectComponentDto { Id = Guid.NewGuid(), Name = "�ɼ�", Description = "���ݲɼ�", Group = "ingest", Version = "1.0.0", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow.AddDays(-9) },
            new ProjectComponentDto { Id = Guid.NewGuid(), Name = "��ϴ", Description = "������ϴ", Group = "clean", Version = "1.0.1", ProjectId = p1.Id, CreatedAt = DateTime.UtcNow.AddDays(-8) }
        });

        // Seed etl connectors
        EtlConns.AddRange(new[]
        {
            new EtlConnectorDto { Id = Guid.NewGuid(), Name = "MySQL->SR", Description = "ȫ��ͬ��", ProjectComponentId = SubProjects.First().Id, DataX = "{}", CreatedAt = DateTime.UtcNow.AddDays(-3) }
        });

        // Seed users
        Users.AddRange(new[]
        {
            new UserDto { Id = Guid.NewGuid(), Account = "admin", Name = "����Ա", Email = "admin@example.com", Role = KiwiStack.Shared.Enums.RoleEnum.Admin, CreatedAt = DateTime.UtcNow.AddDays(-100) },
            new UserDto { Id = Guid.NewGuid(), Account = "user1", Name = "����", Email = "zhangsan@example.com", Role = KiwiStack.Shared.Enums.RoleEnum.User, CreatedAt = DateTime.UtcNow.AddDays(-10) }
        });
    }

    public static KiwiResult<List<T>> Result<T>(IEnumerable<T> list)
        where T : class => new KiwiResult<List<T>> { Data = list.ToList() };
}
