using employeeportal.Data;
using employeeportal.Data.Interfaces;
using employeeportal.Data.Repositories;
using employeeportal.Mcp.Authorization;
using employeeportal.Mcp.Middleware;
using employeeportal.Mcp.Security;
using employeeportal.Services.Interfaces;
using employeeportal.Services.Security;
using employeeportal.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Same DbContext class as the REST API (employeeportal.Data) — this is a separate
// process, so it needs its own DI registration, but it is NOT a different DbContext.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Same repository implementations the REST API uses.
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Same service implementations the REST API uses — this is the shared business logic.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Resolves the authenticated identity (userId, username, roles) straight from
// HttpContext.User — i.e. from the validated JWT's claims. No database access.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Same JWT validation logic as the REST API (employeeportal.Services.Security) —
// same signing key/issuer/audience via configuration, one shared implementation,
// two independent ASP.NET Core hosts each with their own DI container.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtValidationParametersFactory.Create(builder.Configuration);
    });

builder.Services.AddAuthorization();

// Built once at startup by reflecting [McpServerTool] + [RequiredRoles]
// attributes — the single source of truth for which roles may call which tool.
builder.Services.AddSingleton<ToolRoleRegistry>();

// MCP server registration: discover [McpServerToolType] classes in this assembly,
// expose them over Streamable HTTP, and enforce role-based tool visibility
// (tools/list) and execution (tools/call) via the centralized filter. Both
// filters read only JWT role claims (via ICurrentUserService) — no database
// query is made for authorization.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly()
    .WithRequestFilters(filters =>
    {
        filters.AddListToolsFilter(McpRoleAuthorizationFilter.ListToolsFilter);
        filters.AddCallToolFilter(McpRoleAuthorizationFilter.CallToolFilter);
    });

var app = builder.Build();

// MCP-specific middleware pipeline — independent of the REST API's pipeline.
app.UseMiddleware<McpExceptionMiddleware>();

// Authentication before Authorization, same reasoning as the REST API:
// authorization (RequireAuthorization below, and the tool role filters)
// needs HttpContext.User to already be populated from the JWT.
app.UseAuthentication();
app.UseAuthorization();

// RequireAuthorization() rejects a request with no/invalid JWT with a plain
// 401 before it ever reaches MCP's JSON-RPC handling. Role-based tool
// visibility/execution is then enforced per-request by
// McpRoleAuthorizationFilter, registered above.
app.MapMcp("/mcp").RequireAuthorization();

app.Run();
