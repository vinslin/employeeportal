using employeeportal.Entity.Entities;

namespace employeeportal.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user, IEnumerable<string> roles);
}
