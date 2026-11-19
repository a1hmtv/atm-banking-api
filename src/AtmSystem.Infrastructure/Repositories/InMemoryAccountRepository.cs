using AtmSystem.Application.Ports;
using AtmSystem.Core.Accounts;

namespace AtmSystem.Infrastructure.Repositories;

public sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<string, Account> _accounts = new(StringComparer.Ordinal);

    private readonly object _lock = new();

    public Account? FindByNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return null;

        lock (_lock)
        {
            _accounts.TryGetValue(accountNumber, out Account? account);
            return account;
        }
    }

    public void Save(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(account.Number))
            throw new ArgumentException("Account number is required.", nameof(account));

        lock (_lock)
        {
            _accounts[account.Number] = account;
        }
    }
}