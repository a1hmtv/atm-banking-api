namespace AtmSystem.Core.Sessions;

public interface ISessionFactory
{
    Session CreateUser(Guid key, string accountNumber);

    Session CreateAdmin(Guid key);
}