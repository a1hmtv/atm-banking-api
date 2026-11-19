using AtmSystem.Application.Ports;
using AtmSystem.Application.Services;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Operations;
using AtmSystem.Core.Sessions;
using Xunit;

namespace AtmSystem.Tests.Accounts;

public sealed class DepositTests
{
    [Fact]
    public void Deposit_WhenAmountIsValid_SavesAccountWithUpdatedBalance()
    {
        const string accountNumber = "ACC-003";
        const decimal initialBalance = 10m;
        const decimal amount = 15m;
        decimal expectedBalance = initialBalance + amount;

        var sessionKey = Guid.NewGuid();

        var sessionFactory = new SessionFactory();
        Session session = sessionFactory.CreateUser(sessionKey, accountNumber);

        var account = new Account(accountNumber, "0000", initialBalance);

        var sessions = new FakeSessionRepository(session);
        var accounts = new FakeAccountRepository(account);
        var operations = new FakeOperationRepository();

        var service = new AtmService(sessions, accounts, operations);

        service.Deposit(sessionKey, amount);

        Assert.Equal(1, accounts.SaveCalls);
        Assert.NotNull(accounts.LastSavedAccount);
        Assert.NotNull(accounts.LastSavedAccount);
        Assert.Equal(accountNumber, accounts.LastSavedAccount.Number);

        Assert.Equal(expectedBalance, accounts.LastSavedAccount.Balance);

        Assert.Single(operations.Added);
        OperationRecord record = operations.Added[0];

        Assert.Equal(accountNumber, record.AccountNumber);
        Assert.Equal(OperationType.Deposit, record.Type);
        Assert.Equal(amount, record.Amount);
    }

    private sealed class FakeSessionRepository : ISessionRepository
    {
        private readonly Session? _session;

        public FakeSessionRepository(Session? session)
        {
            _session = session;
        }

        public Session? FindByKey(Guid sessionKey)
        {
            return _session is not null && _session.Key == sessionKey ? _session : null;
        }

        public void Save(Session session) { }
    }

    private sealed class FakeAccountRepository : IAccountRepository
    {
        private readonly Account? _account;

        public FakeAccountRepository(Account? account)
        {
            _account = account;
        }

        public int SaveCalls { get; private set; }

        public Account? LastSavedAccount { get; private set; }

        public Account? FindByNumber(string accountNumber)
        {
            return _account is not null && _account.Number == accountNumber ? _account : null;
        }

        public void Save(Account account)
        {
            SaveCalls++;
            LastSavedAccount = account;
        }
    }

    private sealed class FakeOperationRepository : IOperationRepository
    {
        public List<OperationRecord> Added { get; } = new();

        public void Add(OperationRecord record)
        {
            Added.Add(record);
        }

        public IReadOnlyCollection<OperationRecord> GetByAccountNumber(string accountNumber)
        {
            return Added.Where(x => x.AccountNumber == accountNumber).ToArray();
        }
    }
}