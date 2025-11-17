namespace Email.Api.Contracts;

public sealed record GenerateTokenRequest(string UserName, string? Email, string? Role);

public sealed record TokenResponse(string AccessToken, DateTime ExpiresAtUtc);

