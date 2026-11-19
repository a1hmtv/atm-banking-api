namespace AtmSystem.WebApi.Dtos.Accounts;

public sealed record CreateAccountRequest(
    Guid AdminSessionKey,
    string AccountNumber,
    string Pin,
    decimal InitialBalance);