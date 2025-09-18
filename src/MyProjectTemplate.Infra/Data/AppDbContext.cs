using Microsoft.EntityFrameworkCore;

using MyProjectTemplate.Domain.Entities;

namespace MyProjectTemplate.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Example> Examples { get; set; }
}