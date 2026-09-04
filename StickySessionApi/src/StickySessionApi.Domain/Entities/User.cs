namespace StickySessionApi.Domain.Entities;

public sealed class User
{
    public required int Id { get; init; }
    public required string Username { get; init; }
}
