namespace AtmSystem.WebApi.Dtos.Sessions;

public sealed record CreateUserSessionRequest(string AccountNumber, string Pin);