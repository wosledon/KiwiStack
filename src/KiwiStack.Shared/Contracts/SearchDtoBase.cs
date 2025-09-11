namespace KiwiStack.Shared.Contracts;

public class SearchDtoBase
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}