using AtmSystem.Application.Ports;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Operations;
using AtmSystem.Core.Sessions;

namespace AtmSystem.Application.Services;

public sealed class AtmService : IAtmService
{
    private readonly ISessionRepository _sessions;
    private readonly IAccountRepository _accounts;
    private readonly IOperationRepository _operations;

    public AtmService(ISessionRepository sessions, IAccountRepository accounts, IOperationRepository operations)
    {
        _sessions = sessions;
        _accounts = accounts;
        _operations = operations;
    }

    public void Deposit(Guid sessionKey, decimal amount)
    {
        Session session = RequireUserSession(sessionKey);
        Account account = RequireAccount(session);

        account.Deposit(amount);
        _accounts.Save(account);

        _operations.Add(new OperationRecord(account.Number, OperationType.Deposit, amount, DateTimeOffset.UtcNow));
    }

    public void Withdraw(Guid sessionKey, decimal amount)
    {
        Session session = RequireUserSession(sessionKey);
        Account account = RequireAccount(session);

        account.Withdraw(amount);
        _accounts.Save(account);

        _operations.Add(new OperationRecord(account.Number, OperationType.Withdraw, amount, DateTimeOffset.UtcNow));
    }

    public decimal GetBalance(Guid sessionKey)
    {
        Session session = RequireUserSession(sessionKey);

        Account account = RequireAccount(session);

        return account.Balance;
    }

    public IReadOnlyCollection<OperationRecord> GetHistory(Guid sessionKey)
    {
        Session session = RequireUserSession(sessionKey);

        string accountNumber = session.AccountNumber
                               ?? throw new InvalidOperationException("Session does not contain account number.");

        return _operations.GetByAccountNumber(accountNumber);
    }

    private Session RequireUserSession(Guid sessionKey)
    {
        Session? session = _sessions.FindByKey(sessionKey);
        if (session is null) throw new UnauthorizedAccessException("Session not found.");
        if (session.Role != SessionRole.User) throw new UnauthorizedAccessException("Not a user session.");
        if (string.IsNullOrWhiteSpace(session.AccountNumber))
            throw new UnauthorizedAccessException("User session has no account.");
        return session;
    }

    private Account RequireAccount(Session session)
    {
        string accountNumber = session.AccountNumber ??
                               throw new InvalidOperationException("Session does not contain account number.");

        Account? account = _accounts.FindByNumber(accountNumber);

        if (account is null)
            throw new InvalidOperationException("Account not found.");

        return account;
    }
}