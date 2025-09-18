using Dodo.Primitives;

namespace MyProjectTemplate.Core.Base;

public class BaseEntity
{
    public Uuid Id { get; set; } = Uuid.CreateVersion7();
    public bool Active { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}