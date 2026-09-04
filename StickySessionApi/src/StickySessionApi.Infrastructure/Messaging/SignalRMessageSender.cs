using Microsoft.AspNetCore.SignalR;
using StickySessionApi.Application.Messaging;

namespace StickySessionApi.Infrastructure.Messaging;

public sealed class SignalRMessageSender(IHubContext<MessagingHub> hubContext) : IMessageSender
{
    public Task SendToUserAsync(int userId, string message, CancellationToken cancellationToken = default) =>
        hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", message, cancellationToken);
}
