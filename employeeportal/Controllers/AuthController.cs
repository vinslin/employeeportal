using employeeportal.Entity.DTOs;
using employeeportal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace employeeportal.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
        {
            // Deliberately generic: does not reveal whether the username
            // exists, whether the password was wrong, or whether the
            // account is inactive.
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(result);
    }
}
