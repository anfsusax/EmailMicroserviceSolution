using System.Text.RegularExpressions;

namespace Email.Domain.ValueObjects;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("O endereço de e-mail é obrigatório.", nameof(value));
        }

        if (!EmailRegex.IsMatch(value))
        {
            throw new ArgumentException("O endereço de e-mail é inválido.", nameof(value));
        }

        return new EmailAddress(value.Trim());
    }

    public bool Equals(EmailAddress? other) => other is not null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is EmailAddress emailAddress && Equals(emailAddress);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Value;
}

