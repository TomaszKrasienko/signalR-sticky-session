namespace StickySessionApi.Application.Messaging;

public interface IMessageSender
{
    Task SendToUserAsync(int userId, string message, CancellationToken cancellationToken = default);
}
