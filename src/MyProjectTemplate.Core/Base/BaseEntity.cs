using Dodo.Primitives;

namespace MyProjectTemplate.Core.Base;

public class BaseEntity
{
    public Uuid Id { get; private set; } = Uuid.CreateVersion7();
    public bool Active { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
}