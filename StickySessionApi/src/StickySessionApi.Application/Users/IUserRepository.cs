using StickySessionApi.Domain.Entities;

namespace StickySessionApi.Application.Users;

public interface IUserRepository
{
    User? GetById(int id);
    IReadOnlyCollection<User> GetAll();
    User Add(string username);
}
