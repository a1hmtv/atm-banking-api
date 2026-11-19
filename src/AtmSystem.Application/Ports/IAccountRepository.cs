using AtmSystem.Core.Accounts;

namespace AtmSystem.Application.Ports;

public interface IAccountRepository
{
    Account? FindByNumber(string accountNumber);

    void Save(Account account);
}