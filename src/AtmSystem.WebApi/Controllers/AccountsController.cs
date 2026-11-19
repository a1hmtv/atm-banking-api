using AtmSystem.Application.Ports;
using AtmSystem.Application.Services;
using AtmSystem.Core.Accounts;
using AtmSystem.Core.Operations;
using AtmSystem.Core.Sessions;
using AtmSystem.WebApi.Dtos.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace AtmSystem.WebApi.Controllers;

[ApiController]
[Route("accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAtmService _atm;
    private readonly IAccountRepository _accounts;
    private readonly ISessionRepository _sessions;

    public AccountsController(IAtmService atm, IAccountRepository accounts, ISessionRepository sessions)
    {
        _atm = atm;
        _accounts = accounts;
        _sessions = sessions;
    }

    [HttpPost("create")]
    public IActionResult CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            RequireAdminSession(request.AdminSessionKey);

            if (string.IsNullOrWhiteSpace(request.AccountNumber))
                return BadRequest("AccountNumber is required.");

            if (string.IsNullOrWhiteSpace(request.Pin))
                return BadRequest("Pin is required.");

            if (request.InitialBalance < 0)
                return BadRequest("InitialBalance must be >= 0.");

            Account? existing = _accounts.FindByNumber(request.AccountNumber);
            if (existing is not null)
                return BadRequest("Account already exists.");

            var account = new Account(request.AccountNumber, request.Pin, request.InitialBalance);
            _accounts.Save(account);

            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("balance")]
    public ActionResult<GetBalanceResponse> GetBalance([FromQuery] Guid sessionKey)
    {
        try
        {
            decimal balance = _atm.GetBalance(sessionKey);
            return Ok(new GetBalanceResponse(balance));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("deposit")]
    public IActionResult Deposit([FromBody] DepositRequest request)
    {
        try
        {
            _atm.Deposit(request.SessionKey, request.Amount);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("withdraw")]
    public IActionResult Withdraw([FromBody] WithdrawRequest request)
    {
        try
        {
            _atm.Withdraw(request.SessionKey, request.Amount);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history")]
    public ActionResult<GetHistoryResponse> GetHistory([FromQuery] Guid sessionKey)
    {
        try
        {
            IReadOnlyCollection<OperationRecord> history = _atm.GetHistory(sessionKey);

            OperationItemDto[] items = history
                .Select(x => new OperationItemDto(x.Type.ToString(), x.Amount, x.Timestamp))
                .ToArray();

            return Ok(new GetHistoryResponse(items));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private void RequireAdminSession(Guid sessionKey)
    {
        Session? session = _sessions.FindByKey(sessionKey);
        if (session is null) throw new UnauthorizedAccessException("Session not found.");
        if (session.Role != SessionRole.Admin) throw new UnauthorizedAccessException("Not an admin session.");
    }
}
