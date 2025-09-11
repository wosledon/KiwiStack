using System;
using KiwiStack.Shared.Enums;

namespace KiwiStack.Shared.Dtos.DatabaseConnection;

public class DatabaseConnectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DbTypeEnum DbType { get; set; } = DbTypeEnum.MySql;
    public string Host { get; set; } = string.Empty;
    public int? Port { get; set; }
    public string Username { get; set; } = string.Empty;
    // Password intentionally omitted from read DTO
    public string Database { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
}
