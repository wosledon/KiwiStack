using System;
using KiwiStack.Api.Data;
using KiwiStack.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace KiwiStack.Api.Services;

public class UnitOfWork(
    KiwiDbContext db
) : IScopedService
{
    private readonly KiwiDbContext _db = db;

    public IQueryable<T> Q<T>() where T : EntityBase => _db.Set<T>().AsQueryable().AsNoTracking();

    public KiwiDbContext W => _db;

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken) > 0;
    }
}
