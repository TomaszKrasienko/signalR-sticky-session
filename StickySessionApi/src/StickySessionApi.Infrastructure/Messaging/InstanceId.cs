namespace StickySessionApi.Infrastructure.Messaging;

public sealed class InstanceId
{
    public Guid Value { get; } = Guid.NewGuid();
}
