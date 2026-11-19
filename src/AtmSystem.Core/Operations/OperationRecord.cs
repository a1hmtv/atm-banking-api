namespace AtmSystem.Core.Operations;

public sealed class OperationRecord
{
    public OperationRecord(string accountNumber, OperationType type, decimal amount, DateTimeOffset timestamp)
    {
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        Timestamp = timestamp;
    }

    public string AccountNumber { get; }

    public OperationType Type { get; }

    public decimal Amount { get; }

    public DateTimeOffset Timestamp { get; }
}