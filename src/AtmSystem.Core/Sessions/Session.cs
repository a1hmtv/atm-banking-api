namespace AtmSystem.Core.Sessions;

public sealed class Session
{
    internal Session(Guid key, SessionRole role, string? accountNumber)
    {
        Key = key;
        Role = role;
        AccountNumber = accountNumber;
    }

    public Guid Key { get; }

    public SessionRole Role { get; }

    public string? AccountNumber { get; }
}