namespace KiwiStack.Shared.Contracts;

public interface IRequestDto
{

}

public class RequestPageDtoBase : IRequestDto
{
    public string Search { get; set; } = string.Empty;

    public int Page { get; set; }
    public int PageSize { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}

public class RequestDtoBase : IRequestDto
{
    public Guid Id { get; set; }
}