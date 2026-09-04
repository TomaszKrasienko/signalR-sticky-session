using StickySessionApi.Application.Users;

namespace StickySessionApi.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", (IUserRepository repository) => repository.GetAll());
    }
}
