namespace employeeportal.Mcp.Authorization;

/// <summary>
/// Declares which JWT roles may use a tool method. Metadata only — no
/// authorization logic here. Enforcement is centralized in
/// McpRoleAuthorizationFilter, which reads this attribute via
/// ToolRoleRegistry for every tools/list and tools/call request.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequiredRolesAttribute : Attribute
{
    public IReadOnlyCollection<string> Roles { get; }

    public RequiredRolesAttribute(params string[] roles)
    {
        Roles = roles;
    }
}
