namespace AtmSystem.WebApi.Dtos.Accounts;

public sealed record DepositRequest(Guid SessionKey, decimal Amount);