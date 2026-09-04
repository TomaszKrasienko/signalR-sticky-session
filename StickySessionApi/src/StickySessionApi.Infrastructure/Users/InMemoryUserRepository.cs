using System.Collections.Concurrent;
using StickySessionApi.Application.Users;
using StickySessionApi.Domain.Entities;

namespace StickySessionApi.Infrastructure.Users;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId;

    public InMemoryUserRepository()
    {
        Add("user1");
        Add("user2");
    }

    public User? GetById(int id) => _users.GetValueOrDefault(id);

    public IReadOnlyCollection<User> GetAll() => _users.Values.ToList();

    public User Add(string username)
    {
        var id = Interlocked.Increment(ref _nextId);
        var user = new User { Id = id, Username = username };
        _users[id] = user;
        return user;
    }
}
