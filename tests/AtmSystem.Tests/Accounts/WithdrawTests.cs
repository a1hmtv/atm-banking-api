using AtmSystem.Application.Ports;
using AtmSystem.Application.Services;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Operations;
using AtmSystem.Core.Sessions;
using Xunit;

namespace AtmSystem.Tests.Accounts;

public sealed class WithdrawTests
{
    [Fact]
    public void Withdraw_WhenBalanceEnough_SavesAccountWithUpdatedBalance()
    {
        const string accountNumber = "ACC-001";
        const decimal initialBalance = 200m;
        const decimal amount = 50m;
        decimal expectedBalance = initialBalance - amount;

        var sessionKey = Guid.NewGuid();

        var sessionFactory = new SessionFactory();
        Session userSession = sessionFactory.CreateUser(sessionKey, accountNumber);

        var account = new Account(accountNumber, "0000", initialBalance);

        var sessions = new FakeSessionRepository(userSession);
        var accounts = new FakeAccountRepository(account);
        var operations = new FakeOperationRepository();

        var service = new AtmService(sessions, accounts, operations);

        service.Withdraw(sessionKey, amount);

        Assert.Equal(expectedBalance, account.Balance);
        Assert.Equal(1, accounts.SaveCalls);
        Assert.Same(account, accounts.LastSavedAccount);

        Assert.Single(operations.Added);
        Assert.Equal(accountNumber, operations.Added[0].AccountNumber);
        Assert.Equal(OperationType.Withdraw, operations.Added[0].Type);
        Assert.Equal(amount, operations.Added[0].Amount);
    }

    [Fact]
    public void Withdraw_WhenBalanceNotEnough_ReturnsError_AndDoesNotSave()
    {
        const string accountNumber = "ACC-002";
        const decimal initialBalance = 10m;
        const decimal amount = 100m;

        var sessionKey = Guid.NewGuid();

        var sessionFactory = new SessionFactory();
        Session userSession = sessionFactory.CreateUser(sessionKey, accountNumber);

        var account = new Account(accountNumber, "0000", initialBalance);

        var sessions = new FakeSessionRepository(userSession);
        var accounts = new FakeAccountRepository(account);
        var operations = new FakeOperationRepository();

        var service = new AtmService(sessions, accounts, operations);

        Assert.Throws<InvalidOperationException>(() => service.Withdraw(sessionKey, amount));

        Assert.Equal(initialBalance, account.Balance);
        Assert.Equal(0, accounts.SaveCalls);
        Assert.Empty(operations.Added);
    }

    [Fact]
    public void Deposit_WhenAmountValid_SavesAccountWithUpdatedBalance()
    {
        const string accountNumber = "ACC-003";
        const decimal initialBalance = 10m;
        const decimal amount = 15m;
        decimal expectedBalance = initialBalance + amount;

        var sessionKey = Guid.NewGuid();

        var sessionFactory = new SessionFactory();
        Session userSession = sessionFactory.CreateUser(sessionKey, accountNumber);

        var account = new Account(accountNumber, "0000", initialBalance);

        var sessions = new FakeSessionRepository(userSession);
        var accounts = new FakeAccountRepository(account);
        var operations = new FakeOperationRepository();

        var service = new AtmService(sessions, accounts, operations);

        service.Deposit(sessionKey, amount);

        Assert.Equal(expectedBalance, account.Balance);
        Assert.Equal(1, accounts.SaveCalls);
        Assert.Same(account, accounts.LastSavedAccount);

        Assert.Single(operations.Added);
        Assert.Equal(accountNumber, operations.Added[0].AccountNumber);
        Assert.Equal(OperationType.Deposit, operations.Added[0].Type);
        Assert.Equal(amount, operations.Added[0].Amount);
    }

    private sealed class FakeAccountRepository : IAccountRepository
    {
        private readonly Account? _accountToReturn;

        public FakeAccountRepository(Account? accountToReturn)
        {
            _accountToReturn = accountToReturn;
        }

        public int SaveCalls { get; private set; }

        public Account? LastSavedAccount { get; private set; }

        public Account? FindByNumber(string accountNumber)
        {
            return _accountToReturn is not null && _accountToReturn.Number == accountNumber
                ? _accountToReturn
                : null;
        }

        public void Save(Account account)
        {
            SaveCalls++;
            LastSavedAccount = account;
        }
    }

    private sealed class FakeSessionRepository : ISessionRepository
    {
        private readonly Session? _sessionToReturn;

        public FakeSessionRepository(Session? sessionToReturn)
        {
            _sessionToReturn = sessionToReturn;
        }

        public int SaveCalls { get; private set; }

        public Session? LastSavedSession { get; private set; }

        public Session? FindByKey(Guid sessionKey)
        {
            return _sessionToReturn is not null && _sessionToReturn.Key == sessionKey
                ? _sessionToReturn
                : null;
        }

        public void Save(Session session)
        {
            SaveCalls++;
            LastSavedSession = session;
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