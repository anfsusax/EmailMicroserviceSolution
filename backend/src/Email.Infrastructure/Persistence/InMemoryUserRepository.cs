using System.Collections.Concurrent;
using Email.Application.Abstractions;
using Email.Domain.Entities;

namespace Email.Infrastructure.Persistence;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<string, Guid> _userNameIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Guid> _emailIndex = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult<User?>(null);

        if (!_userNameIndex.TryGetValue(userName, out var userId))
            return Task.FromResult<User?>(null);

        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult<User?>(null);

        var normalizedEmail = email.ToLowerInvariant();
        if (!_emailIndex.TryGetValue(normalizedEmail, out var userId))
            return Task.FromResult<User?>(null);

        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (_users.TryAdd(user.Id, user))
        {
            _userNameIndex[user.UserName] = user.Id;
            _emailIndex[user.Email] = user.Id;
        }
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (_users.TryGetValue(user.Id, out var existingUser))
        {
            // Atualizar índices se email ou username mudaram
            if (existingUser.Email != user.Email)
            {
                _emailIndex.TryRemove(existingUser.Email, out _);
                _emailIndex[user.Email] = user.Id;
            }

            if (existingUser.UserName != user.UserName)
            {
                _userNameIndex.TryRemove(existingUser.UserName, out _);
                _userNameIndex[user.UserName] = user.Id;
            }

            _users[user.Id] = user;
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult(false);

        return Task.FromResult(_userNameIndex.ContainsKey(userName));
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Task.FromResult(false);

        return Task.FromResult(_emailIndex.ContainsKey(email.ToLowerInvariant()));
    }
}

