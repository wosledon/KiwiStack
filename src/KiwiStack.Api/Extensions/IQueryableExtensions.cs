using System;
using KiwiStack.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace KiwiStack.Api.Extensions;

public static class IQueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int page, int pageSize)
    {
        var total = await source.CountAsync();
        var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, page, pageSize, total);
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Func<IQueryable<T>, IQueryable<T>> predicate)
    {
        if (condition)
        {
            return predicate(source);
        }

        return source;
    }
}