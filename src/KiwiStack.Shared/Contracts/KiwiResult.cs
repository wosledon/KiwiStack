using System.Collections;
using System.Net;

namespace KiwiStack.Shared.Contracts;

public class KiwiResult<T>
    where T:class
{
    public T? Data { get; set; } = default;

    private int? _total;

    public int Total
    {
        get
        {
            if (_total.HasValue) return _total.Value;

            if (Data is IPagedList paged)
            {
                return paged.Total;
            }

            if(Data is IList list)
            {
                return list.Count;
            }

            return 0;
        }
        set
        {
            _total = value;
        }
    }
}