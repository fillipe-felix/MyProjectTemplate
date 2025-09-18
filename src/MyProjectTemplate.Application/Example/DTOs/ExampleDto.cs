using Dodo.Primitives;

using MyProjectTemplate.Domain.Enums;

namespace MyProjectTemplate.Application.Example.DTOs;

public class ExampleDto
{
    public Uuid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get;  set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Difficulty Difficulty { get;  set; }

    public ExampleDto()
    {
        
    }
    
    public ExampleDto ToDTO(Domain.Entities.Example example)
    {
        return new ExampleDto
        {
            Id = example.Id,
            Name = example.Name,
            Description = example.Description,
            Date = example.Date,
            Location = example.Location,
            Latitude = example.Latitude,
            Longitude = example.Longitude,
            Difficulty = example.Difficulty
        };
    }
}
