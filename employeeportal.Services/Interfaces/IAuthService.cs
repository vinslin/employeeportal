using employeeportal.Entity.DTOs;

namespace employeeportal.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Returns null for invalid username, wrong password, or an inactive
    /// account — callers must not distinguish these cases in the response.
    /// </summary>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
