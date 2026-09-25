using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace employeeportal.Services.Security;

/// <summary>
/// Builds the JWT bearer TokenValidationParameters from the "Jwt" configuration
/// section. Both the REST API host and the MCP host call this so there is a
/// single JWT validation implementation shared across the two processes,
/// instead of two independently-maintained copies.
/// </summary>
public static class JwtValidationParametersFactory
{
    public static TokenValidationParameters Create(IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException(
                "Jwt:SecretKey is not configured. Set it via appsettings.Development.json " +
                "locally, or via environment variable/secret manager in other environments.");

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
        };
    }
}
