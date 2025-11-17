using Email.Application.Abstractions;
using Email.Domain.Entities;

namespace Email.Application.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string userName, string email, string password, string? role = null, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(string userName, string password, CancellationToken cancellationToken = default);
}

public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public User? User { get; init; }
    public string? ErrorMessage { get; init; }

    public static AuthResult SuccessResult(string token, DateTime expiresAt, User user)
    {
        return new AuthResult
        {
            Success = true,
            Token = token,
            ExpiresAt = expiresAt,
            User = user
        };
    }

    public static AuthResult FailureResult(string errorMessage)
    {
        return new AuthResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}

internal sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> RegisterAsync(string userName, string email, string password, string? role = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return AuthResult.FailureResult("UserName é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            return AuthResult.FailureResult("Email é obrigatório.");

        if (string.IsNullOrWhiteSpace(password))
            return AuthResult.FailureResult("Password é obrigatório.");

        if (password.Length < 6)
            return AuthResult.FailureResult("Password deve ter pelo menos 6 caracteres.");

        // Verificar se usuário já existe
        if (await _userRepository.ExistsByUserNameAsync(userName, cancellationToken))
            return AuthResult.FailureResult("UserName já está em uso.");

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            return AuthResult.FailureResult("Email já está em uso.");

        // Criar usuário
        var passwordHash = _passwordHasher.HashPassword(password);
        var user = User.Create(userName, email, passwordHash, role);
        await _userRepository.CreateAsync(user, cancellationToken);

        return AuthResult.SuccessResult(string.Empty, DateTime.UtcNow, user);
    }

    public async Task<AuthResult> LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return AuthResult.FailureResult("UserName é obrigatório.");

        if (string.IsNullOrWhiteSpace(password))
            return AuthResult.FailureResult("Password é obrigatório.");

        // Buscar usuário
        var user = await _userRepository.GetByUserNameAsync(userName, cancellationToken);
        if (user is null)
            return AuthResult.FailureResult("Credenciais inválidas.");

        if (!user.IsActive)
            return AuthResult.FailureResult("Usuário inativo.");

        // Verificar senha
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return AuthResult.FailureResult("Credenciais inválidas.");

        return AuthResult.SuccessResult(string.Empty, DateTime.UtcNow, user);
    }
}

