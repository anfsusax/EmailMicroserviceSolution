namespace Email.Api.Contracts;

public sealed record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string? Role = null
);

public sealed record LoginRequest(
    string UserName,
    string Password
);

public sealed record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string? Role,
    DateTime CreatedAt
);

