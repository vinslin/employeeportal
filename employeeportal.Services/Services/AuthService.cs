using employeeportal.Data.Interfaces;
using employeeportal.Entity.DTOs;
using employeeportal.Services.Interfaces;

namespace employeeportal.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUserNameWithRolesAsync(request.UserName);

        // Same failure path for "no such user", "wrong password", and
        // "inactive account" — the caller (AuthController) must not be able
        // to tell these apart, so it can't leak whether a username exists.
        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return null;
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (token, expiresAt) = _jwtTokenService.CreateToken(user, roles);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            UserName = user.UserName,
            Roles = roles,
        };
    }
}
