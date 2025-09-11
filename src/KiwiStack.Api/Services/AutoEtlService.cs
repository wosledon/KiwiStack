using KiwiStack.Shared.Contracts;
using KiwiStack.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace KiwiStack.Api.Services;

public class AutoEtlService (
    UnitOfWork db
    ): IScopedService
{
    private readonly UnitOfWork _db = db;

}
