namespace employeeportal.Mcp.Middleware;

public class McpExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<McpExceptionMiddleware> _logger;

    public McpExceptionMiddleware(RequestDelegate next, ILogger<McpExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing MCP request {Path}", context.Request.Path);

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred while processing the MCP request."
                });
            }
        }
    }
}
