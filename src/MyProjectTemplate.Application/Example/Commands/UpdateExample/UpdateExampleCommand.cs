using Dodo.Primitives;

using MediatR;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Enums;

namespace MyProjectTemplate.Application.Example.Commands.UpdateExample;

public class UpdateExampleCommand : IRequest<BaseResult>
{
    public Uuid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Difficulty Difficulty { get; set; }
}
