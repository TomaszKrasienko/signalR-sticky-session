using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StickySessionApi.Infrastructure.Messaging;

public static class SignalRServiceExtensions
{
    public static ISignalRServerBuilder AddMessagingSignalR(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection("Redis"));
        var redisOptions = configuration.GetSection("Redis").Get<RedisOptions>() ?? new RedisOptions();

        var signalRBuilder = services.AddSignalR();

        if (redisOptions.Enabled)
        {
            signalRBuilder.AddStackExchangeRedis(redisOptions.ConnectionString);
        }

        return signalRBuilder;
    }
}
