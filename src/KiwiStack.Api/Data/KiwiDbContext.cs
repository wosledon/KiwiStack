using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using KiwiStack.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace KiwiStack.Api.Data;

public class KiwiDbContext : DbContext
{
    public KiwiDbContext(DbContextOptions<KiwiDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var type in GetEntityTypes())
        {
            if(modelBuilder.Model.FindEntityType(type) is not null)
                continue;
            
            modelBuilder.Model.AddEntityType(type);

            var entity = modelBuilder.Entity(type);

            var parameter = Expression.Parameter(type, "e");
            var property = Expression.PropertyOrField(parameter, nameof(EntityBase.IsDeleted));
            var compare = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            entity.HasQueryFilter(lambda);
        }

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {
        ConfigureAuditProperties();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConfigureAuditProperties();
        return await base.SaveChangesAsync(cancellationToken);
    }

    void ConfigureAuditProperties()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is EntityBase && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

        var currentTime = DateTime.Now;

        foreach (var entry in entries)
        {
            var entity = (EntityBase)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = currentTime;
                entity.UpdatedAt = currentTime;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = currentTime;
            }

            if (entry.State == EntityState.Deleted)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = currentTime;
                entry.State = EntityState.Modified; 
            }
        }
    }

    IEnumerable<Type> GetEntityTypes()
    {
        // KiwiStack.Shared中继承自EntityBase的所有实体
        var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(EntityBase).IsAssignableFrom(t))
            .ToList();

        return entityTypes;
    }
}
