namespace AtmSystem.Application.Services;

public interface ISessionService
{
    Guid CreateUserSession(string accountNumber, string pin);

    Guid CreateAdminSession(string inputSystemPassword, string actualSystemPassword);
}