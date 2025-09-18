using MyProjectTemplate.Domain.Entities;
using MyProjectTemplate.Domain.Interfaces;
using MyProjectTemplate.Infra.Data;

namespace MyProjectTemplate.Infra.Repositories;

public class ExampleRepository  : BaseRepository<Example>, IExampleRepository
{

    public ExampleRepository(AppDbContext context) : base(context)
    {
    }
}
