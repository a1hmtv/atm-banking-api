using AtmSystem.Core.Sessions;

namespace AtmSystem.Application.Ports;

public interface ISessionRepository
{
    Session? FindByKey(Guid sessionKey);

    void Save(Session session);
}