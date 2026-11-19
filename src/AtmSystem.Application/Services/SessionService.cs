using AtmSystem.Application.Ports;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Sessions;

namespace AtmSystem.Application.Services;

public sealed class SessionService : ISessionService
{
    private readonly IAccountRepository _accounts;
    private readonly ISessionRepository _sessions;
    private readonly ISessionFactory _sessionFactory;

    public SessionService(
        IAccountRepository accounts,
        ISessionRepository sessions,
        ISessionFactory sessionFactory)
    {
        _accounts = accounts;
        _sessions = sessions;
        _sessionFactory = sessionFactory;
    }

    public Guid CreateUserSession(string accountNumber, string pin)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.", nameof(accountNumber));

        if (string.IsNullOrWhiteSpace(pin))
            throw new ArgumentException("Pin is required.", nameof(pin));

        Account? account = _accounts.FindByNumber(accountNumber);

        if (account is null || !account.VerifyPin(pin))
            throw new UnauthorizedAccessException("Invalid account number or pin.");

        var key = Guid.NewGuid();

        Session session = _sessionFactory.CreateUser(key, accountNumber);

        _sessions.Save(session);
        return key;
    }

    public Guid CreateAdminSession(string inputSystemPassword, string actualSystemPassword)
    {
        if (string.IsNullOrWhiteSpace(actualSystemPassword) || inputSystemPassword != actualSystemPassword)
            throw new UnauthorizedAccessException("Invalid system password.");

        var key = Guid.NewGuid();

        Session session = _sessionFactory.CreateAdmin(key);

        _sessions.Save(session);
        return key;
    }
}
