using MyProjectTemplate.Core.Base;
using MyProjectTemplate.Domain.Enums;

namespace MyProjectTemplate.Domain.Entities;

public class Example : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; }
    public string Location { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public Difficulty Difficulty { get; private set; }

    public Example(
        string name, 
        string description, 
        DateTime date,
        string location, 
        
        Difficulty difficulty, 
        decimal? latitude = null,
        decimal? longitude = null)
    {
        Name = name;
        Description = description;
        Date = date;
        Location = location;
        Difficulty = difficulty;
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public void Update(
        string requestName,
        string requestDescription,
        DateTime requestDate,
        string requestLocation,
        decimal? requestLatitude,
        decimal? requestLongitude,
        Difficulty requestDifficulty)
    {
        Name = requestName;
        Description = requestDescription;
        Date = requestDate;
        Location = requestLocation;
        Latitude = requestLatitude;
        Longitude = requestLongitude;
        Difficulty = requestDifficulty;
    }
}
