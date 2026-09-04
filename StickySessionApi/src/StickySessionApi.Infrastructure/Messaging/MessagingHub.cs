using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace StickySessionApi.Infrastructure.Messaging;

public sealed class MessagingHub(ILogger<MessagingHub> logger, InstanceId instanceId) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation(
            "Instance {InstanceId}: user {UserId} connected (connectionId={ConnectionId})",
            instanceId.Value, userId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? "unknown";
        logger.LogInformation(
            "Instance {InstanceId}: user {UserId} disconnected (connectionId={ConnectionId})",
            instanceId.Value, userId, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
