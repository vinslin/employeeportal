using employeeportal.Entity.Entities;
using employeeportal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace employeeportal.Services.Services;

/// <summary>
/// Thin wrapper around ASP.NET Core Identity's built-in PasswordHasher (PBKDF2,
/// per-password random salt, configurable iteration count). Not a custom
/// hashing implementation — Identity does the actual cryptography.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _identityHasher = new();

    public string HashPassword(string password)
    {
        // The "user" argument is unused by Identity's default hasher
        // implementation; it exists only for extensibility.
        return _identityHasher.HashPassword(user: null!, password: password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _identityHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
