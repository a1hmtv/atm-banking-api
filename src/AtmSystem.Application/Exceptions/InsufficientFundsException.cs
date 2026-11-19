namespace AtmSystem.Application.Exceptions;

public sealed class InsufficientFundsException : Exception
{
    public InsufficientFundsException()
        : base("Insufficient funds.")
    {
    }

    public InsufficientFundsException(string message)
        : base(message)
    {
    }

    public InsufficientFundsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}