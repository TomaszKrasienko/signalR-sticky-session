namespace StickySessionApi.Application.Messaging;

public sealed record SendMessageRequest(int UserId, string Message);
