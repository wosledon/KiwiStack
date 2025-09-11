using System;
using KiwiStack.Shared.Contracts;

namespace KiwiStack.Shared.Dtos.Project;

public class ProjectSearchDto: SearchDtoBase
{
    public string? Keyword { get; set; }
}
