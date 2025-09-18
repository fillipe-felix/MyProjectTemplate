using Microsoft.EntityFrameworkCore;

namespace MyProjectTemplate.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}