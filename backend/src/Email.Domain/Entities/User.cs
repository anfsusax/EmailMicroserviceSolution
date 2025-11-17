namespace Email.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // Para EF Core

    private User(Guid id, string userName, string email, string passwordHash, string? role)
    {
        Id = id;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role ?? "User";
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static User Create(string userName, string email, string passwordHash, string? role = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName não pode ser vazio.", nameof(userName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash não pode ser vazio.", nameof(passwordHash));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email inválido.", nameof(email));

        if (userName.Length < 3)
            throw new ArgumentException("UserName deve ter pelo menos 3 caracteres.", nameof(userName));

        return new User(Guid.NewGuid(), userName, email.ToLowerInvariant(), passwordHash, role);
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Email inválido.", nameof(email));

        Email = email.ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(string? role)
    {
        Role = role ?? "User";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

