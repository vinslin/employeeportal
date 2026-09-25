using employeeportal.Data;
using employeeportal.Data.Interfaces;
using employeeportal.Data.Repositories;
using employeeportal.Mcp.Middleware;
using employeeportal.Services.Interfaces;
using employeeportal.Services.Services;
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

// MCP server registration: discover [McpServerToolType] classes in this assembly
// and expose them over Streamable HTTP.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// MCP-specific middleware pipeline — independent of the REST API's pipeline.
app.UseMiddleware<McpExceptionMiddleware>();

app.MapMcp("/mcp");

app.Run();
