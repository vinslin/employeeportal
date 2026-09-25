using System.Reflection;
using ModelContextProtocol.Server;

namespace employeeportal.Mcp.Authorization;

/// <summary>
/// The single source of truth for "which roles may call this MCP tool".
/// Built once, at startup, by reflecting over every [McpServerToolType]
/// class in this assembly and reading each method's [McpServerTool] name
/// plus its [RequiredRoles] attribute — nobody hand-maintains a second copy
/// of this mapping.
///
/// A tool with no [RequiredRoles] attribute is NOT treated as public;
/// McpRoleAuthorizationFilter fails closed for it.
/// </summary>
public class ToolRoleRegistry
{
    private readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> _toolRoles;

    public ToolRoleRegistry()
        : this(Assembly.GetExecutingAssembly())
    {
    }

    public ToolRoleRegistry(Assembly assembly)
    {
        var map = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.Ordinal);

        var toolTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<McpServerToolTypeAttribute>() is not null);

        foreach (var type in toolTypes)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var toolAttribute = method.GetCustomAttribute<McpServerToolAttribute>();
                if (toolAttribute is null)
                {
                    continue;
                }

                var toolName = !string.IsNullOrWhiteSpace(toolAttribute.Name)
                    ? toolAttribute.Name
                    : method.Name;

                var requiredRoles = method.GetCustomAttribute<RequiredRolesAttribute>();
                if (requiredRoles is not null)
                {
                    map[toolName] = requiredRoles.Roles;
                }
            }
        }

        _toolRoles = map;
    }

    public IReadOnlyCollection<string>? GetAllowedRoles(string toolName)
    {
        return _toolRoles.TryGetValue(toolName, out var roles) ? roles : null;
    }
}
