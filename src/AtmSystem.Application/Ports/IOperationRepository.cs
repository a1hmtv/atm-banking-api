using AtmSystem.Core.Operations;

namespace AtmSystem.Application.Ports;

public interface IOperationRepository
{
    void Add(OperationRecord record);

    IReadOnlyCollection<OperationRecord> GetByAccountNumber(string accountNumber);
}