using MediatR;

using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Enums;

namespace MyProjectTemplate.Application.Example.Commands.CreateExample;

public record CreateExampleCommand : IRequest<BaseResult<CreateExampleResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Difficulty Difficulty { get; set; }
    
    public Domain.Entities.Example ToEntity()
    {
        return new Domain.Entities.Example(Name, Description, Date, Location, Difficulty, Latitude, Longitude);
    }
}
