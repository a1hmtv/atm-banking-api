namespace AtmSystem.WebApi.Dtos.Accounts;

public sealed record OperationItemDto(
    string Type,
    decimal Amount,
    DateTimeOffset Timestamp);