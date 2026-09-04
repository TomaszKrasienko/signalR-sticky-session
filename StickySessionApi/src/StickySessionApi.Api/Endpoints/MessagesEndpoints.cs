using StickySessionApi.Application.Messaging;

namespace StickySessionApi.Api.Endpoints;

public static class MessagesEndpoints
{
    public static void MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/messages", async (SendMessageRequest request, IMessageSender sender, CancellationToken ct) =>
        {
            await sender.SendToUserAsync(request.UserId, request.Message, ct);
            return Results.Accepted();
        });
    }
}
