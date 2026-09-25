using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace employeeportal.Mcp.Security;

/// <summary>
/// Reads the authenticated identity straight from HttpContext.User — i.e.
/// from the claims the JwtBearer middleware already validated out of the
/// incoming JWT. No database access here. Never trusts a user/role value
/// supplied as an MCP tool argument.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? UserName => Principal?.Identity?.Name;

    public IReadOnlyCollection<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
}
