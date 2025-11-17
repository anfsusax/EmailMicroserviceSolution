namespace Email.Domain.ValueObjects;

public sealed class Recipient
{
    private Recipient(string name, EmailAddress address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; }
    public EmailAddress Address { get; }

    public static Recipient Create(string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome do destinatário é obrigatório.", nameof(name));
        }

        return new Recipient(name.Trim(), EmailAddress.Create(address));
    }
}

