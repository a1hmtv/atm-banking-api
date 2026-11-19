namespace AtmSystem.Core.Sessions;

public sealed class SessionFactory : ISessionFactory
{
    public Session CreateUser(Guid key, string accountNumber) => new(key, SessionRole.User, accountNumber);

    public Session CreateAdmin(Guid key) => new(key, SessionRole.Admin, null);
}