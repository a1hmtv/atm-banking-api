using AtmSystem.Application.Ports;
using AtmSystem.Core.Sessions;

namespace AtmSystem.Infrastructure.Repositories;

public sealed class InMemorySessionRepository : ISessionRepository
{
    private readonly Dictionary<Guid, Session> _sessions = new();

    private readonly object _lock = new();

    public Session? FindByKey(Guid sessionKey)
    {
        if (sessionKey == Guid.Empty)
            return null;

        lock (_lock)
        {
            _sessions.TryGetValue(sessionKey, out Session? session);
            return session;
        }
    }

    public void Save(Session session)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.Key == Guid.Empty)
            throw new ArgumentException("Session key must not be empty.", nameof(session));

        lock (_lock)
        {
            _sessions[session.Key] = session;
        }
    }
}