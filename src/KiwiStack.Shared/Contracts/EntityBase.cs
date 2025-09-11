using System;
using System.Collections.Generic;
using System.Linq;
using KiwiStack.Shared.Entities;

namespace KiwiStack.Shared.Contracts;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsDeleted { get; set; } = false;
    public Guid CreatedBy { get; set; }
    public virtual User? CreatedByUser { get; set; }
    public virtual User? UpdatedByUser { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
