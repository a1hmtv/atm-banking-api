using AtmSystem.Application.Ports;
using AtmSystem.Application.Services;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Sessions;
using Xunit;

namespace AtmSystem.Tests.Sessions;

public sealed class AdminSessionTests
{
    [Fact]
    public void CreateAdminSession_WithValidPassword_SavesAdminSession()
    {
        var sessions = new FakeSessionRepository();
        var service = new SessionService(new FakeAccountRepository(), sessions, new SessionFactory());

        Guid key = service.CreateAdminSession("test-password", "test-password");

        Session? session = sessions.FindByKey(key);
        Assert.NotNull(session);
        Assert.Equal(SessionRole.Admin, session.Role);
        Assert.Null(session.AccountNumber);
    }

    [Fact]
    public void CreateAdminSession_WithWrongPassword_RejectsRequest()
    {
        var sessions = new FakeSessionRepository();
        var service = new SessionService(new FakeAccountRepository(), sessions, new SessionFactory());

        Assert.Throws<UnauthorizedAccessException>(() => service.CreateAdminSession("wrong", "test-password"));
        Assert.Null(sessions.Saved);
    }

    [Fact]
    public void CreateAdminSession_WithEmptyConfiguredPassword_RejectsRequest()
    {
        var sessions = new FakeSessionRepository();
        var service = new SessionService(new FakeAccountRepository(), sessions, new SessionFactory());

        Assert.Throws<UnauthorizedAccessException>(() => service.CreateAdminSession("", ""));
        Assert.Null(sessions.Saved);
    }

    private sealed class FakeSessionRepository : ISessionRepository
    {
        public Session? Saved { get; private set; }

        public Session? FindByKey(Guid sessionKey) => Saved?.Key == sessionKey ? Saved : null;

        public void Save(Session session) => Saved = session;
    }

    private sealed class FakeAccountRepository : IAccountRepository
    {
        public Account? FindByNumber(string accountNumber) => null;

        public void Save(Account account) { }
    }
}
