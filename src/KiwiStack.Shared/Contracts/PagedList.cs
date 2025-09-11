using System;

namespace KiwiStack.Shared.Contracts;

public interface IPagedList
{
    int Page { get; }
    int PageSize { get; }
    int Total{ get; }
}

public class PagedList<T> : List<T>, IPagedList
{
    public int Page { get; }
    public int PageSize { get; }

    public int Total { get; set; }

    public PagedList(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        AddRange(items);
        Total = totalCount;
    }
}
