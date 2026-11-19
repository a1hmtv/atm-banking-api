using AtmSystem.Application.Ports;
using AtmSystem.Core.Operations;

namespace AtmSystem.Infrastructure.Repositories;

public sealed class InMemoryOperationRepository : IOperationRepository
{
    private readonly Dictionary<string, List<OperationRecord>> _historyByAccount =
        new(StringComparer.Ordinal);

    private readonly object _lock = new();

    public void Add(OperationRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (string.IsNullOrWhiteSpace(record.AccountNumber))
            throw new ArgumentException("Account number is required.", nameof(record));

        lock (_lock)
        {
            if (!_historyByAccount.TryGetValue(record.AccountNumber, out List<OperationRecord>? list))
            {
                list = new List<OperationRecord>();
                _historyByAccount[record.AccountNumber] = list;
            }

            list.Add(record);
        }
    }

    public IReadOnlyCollection<OperationRecord> GetByAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return Array.Empty<OperationRecord>();

        lock (_lock)
        {
            if (_historyByAccount.TryGetValue(accountNumber, out List<OperationRecord>? list))
            {
                return list.ToArray();
            }

            return Array.Empty<OperationRecord>();
        }
    }
}