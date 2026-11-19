namespace AtmSystem.WebApi.Dtos.Accounts;

public sealed record WithdrawRequest(Guid SessionKey, decimal Amount);