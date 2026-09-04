namespace StickySessionApi.Infrastructure.Messaging;

public sealed class RedisOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
