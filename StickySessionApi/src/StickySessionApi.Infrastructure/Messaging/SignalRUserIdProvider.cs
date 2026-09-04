using Microsoft.AspNetCore.SignalR;

namespace StickySessionApi.Infrastructure.Messaging;

public sealed class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.GetHttpContext()?.Request.Query["userId"];
}
