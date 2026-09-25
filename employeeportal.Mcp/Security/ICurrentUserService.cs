namespace employeeportal.Mcp.Security;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    string? UserName { get; }

    IReadOnlyCollection<string> Roles { get; }
}
