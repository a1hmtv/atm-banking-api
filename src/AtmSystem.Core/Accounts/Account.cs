namespace AtmSystem.Core.Accounts;

public sealed class Account
{
    public Account(string number, string pin, decimal balance)
    {
        Number = number;
        Pin = pin;
        Balance = balance;
    }

    public string Number { get; }

    public decimal Balance { get; private set; }

    private string Pin { get; }

    public bool VerifyPin(string pin) => Pin == pin;

    public void Deposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        if (Balance < amount) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}