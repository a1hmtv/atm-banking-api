using AtmSystem.Core.Operations;

namespace AtmSystem.Application.Services;

public interface IAtmService
{
    void Deposit(Guid sessionKey, decimal amount);

    void Withdraw(Guid sessionKey, decimal amount);

    decimal GetBalance(Guid sessionKey);

    IReadOnlyCollection<OperationRecord> GetHistory(Guid sessionKey);
}