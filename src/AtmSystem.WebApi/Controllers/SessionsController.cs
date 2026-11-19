using AtmSystem.Application.Services;
using AtmSystem.WebApi.Dtos.Sessions;
using AtmSystem.WebApi.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtmSystem.WebApi.Controllers;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IOptions<SystemPasswordOptions> _systemPasswordOptions;

    public SessionsController(
        ISessionService sessionService,
        IOptions<SystemPasswordOptions> systemPasswordOptions)
    {
        _sessionService = sessionService;
        _systemPasswordOptions = systemPasswordOptions;
    }

    [HttpPost("user")]
    public ActionResult<CreateSessionResponse> CreateUserSession([FromBody] CreateUserSessionRequest request)
    {
        try
        {
            Guid key = _sessionService.CreateUserSession(request.AccountNumber, request.Pin);
            return Ok(new CreateSessionResponse(key));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("admin")]
    public ActionResult<CreateSessionResponse> CreateAdminSession([FromBody] CreateAdminSessionRequest request)
    {
        try
        {
            string actualSystemPassword = _systemPasswordOptions.Value.Value;

            Guid key = _sessionService.CreateAdminSession(
                request.SystemPassword,
                actualSystemPassword);

            return Ok(new CreateSessionResponse(key));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
