using System.ComponentModel.DataAnnotations;
using ModelContextProtocol;

namespace employeeportal.Mcp.Tools;

/// <summary>
/// Runs the DataAnnotations already declared on the existing DTOs (Entity project).
/// MVC controllers get this for free via model binding; MCP tools invoke DTOs directly,
/// so this replays the same, already-existing rules instead of re-implementing them.
/// </summary>
internal static class DtoValidator
{
    public static void ValidateOrThrow(object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(dto, context, results, validateAllProperties: true))
        {
            var message = string.Join(" ", results.Select(r => r.ErrorMessage));
            throw new McpException(message);
        }
    }
}
