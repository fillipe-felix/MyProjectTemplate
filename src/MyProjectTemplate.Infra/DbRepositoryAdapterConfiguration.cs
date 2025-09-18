using System.ComponentModel.DataAnnotations;

namespace MyProjectTemplate.Infra;

public class DbRepositoryAdapterConfiguration
{
    [Required]
    public string SqlConnectionString { get; set; }
}