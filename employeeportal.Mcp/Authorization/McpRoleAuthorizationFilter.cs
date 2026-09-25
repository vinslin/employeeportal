using employeeportal.Mcp.Security;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace employeeportal.Mcp.Authorization;

/// <summary>
/// The centralized MCP authorization boundary. No [McpServerTool] method
/// contains any role-checking code — every check happens here, in exactly
/// two places, using ONLY the role claims already present on the validated
/// JWT (HttpContext.User via ICurrentUserService). No database query is
/// made for either check.
///
///   1. ListToolsFilter — tools/list VISIBILITY. Runs after the inner
///      handler produces the full tool list, then removes any tool the
///      caller's role(s) don't allow. This only affects what a client is
///      told exists; it grants no access by itself.
///
///   2. CallToolFilter — tools/call EXECUTION. This is the real security
///      boundary. It runs BEFORE the inner handler (the actual tool method)
///      is invoked, and only calls `next` — which is what ultimately runs
///      the tool body — if the role check passes. A client that manually
///      asks for a tool tools/list would have hidden is checked exactly the
///      same way here. There is no path to tool execution that skips this
///      filter.
/// </summary>
public static class McpRoleAuthorizationFilter
{
    public static McpRequestFilter<ListToolsRequestParams, ListToolsResult> ListToolsFilter =>
        next => async (context, cancellationToken) =>
        {
            var result = await next(context, cancellationToken);

            var services = context.Services;
            if (services is null)
            {
                result.Tools = new List<Tool>();
                return result;
            }

            var currentUser = services.GetRequiredService<ICurrentUserService>();
            if (!currentUser.IsAuthenticated || currentUser.Roles.Count == 0)
            {
                result.Tools = new List<Tool>();
                return result;
            }

            var registry = services.GetRequiredService<ToolRoleRegistry>();

            result.Tools = result.Tools
                .Where(tool => IsAllowed(tool.Name, registry, currentUser.Roles))
                .ToList();

            return result;
        };

    public static McpRequestFilter<CallToolRequestParams, CallToolResult> CallToolFilter =>
        next => async (context, cancellationToken) =>
        {
            var toolName = context.Params?.Name;
            var services = context.Services;

            if (string.IsNullOrEmpty(toolName) || services is null)
            {
                return Deny("The requested tool could not be identified.");
            }

            var registry = services.GetRequiredService<ToolRoleRegistry>();
            var allowedRoles = registry.GetAllowedRoles(toolName);

            // Fail closed: a tool with no recorded [RequiredRoles] is never
            // executable through this server, not even by SuperAdmin.
            if (allowedRoles is null)
            {
                return Deny("This tool is not available.");
            }

            var currentUser = services.GetRequiredService<ICurrentUserService>();
            if (!currentUser.IsAuthenticated)
            {
                return Deny("Authentication is required to use this tool.");
            }

            if (!currentUser.Roles.Any(role => allowedRoles.Contains(role)))
            {
                return Deny($"Your role does not have access to this tool. Required role(s): {string.Join(", ", allowedRoles)}.");
            }

            // Only now does the actual tool method run.
            return await next(context, cancellationToken);
        };

    private static bool IsAllowed(
        string toolName,
        ToolRoleRegistry registry,
        IReadOnlyCollection<string> userRoles)
    {
        var allowedRoles = registry.GetAllowedRoles(toolName);
        return allowedRoles is not null && userRoles.Any(role => allowedRoles.Contains(role));
    }

    private static CallToolResult Deny(string message) => new()
    {
        IsError = true,
        Content = new List<ContentBlock> { new TextContentBlock { Text = message } },
    };
}
