using System.ComponentModel;
using employeeportal.Entity.DTOs;
using employeeportal.Mcp.Authorization;
using employeeportal.Services.Interfaces;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace employeeportal.Mcp.Tools;

[McpServerToolType]
public class EmployeeTools
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeTools> _logger;

    public EmployeeTools(IEmployeeService employeeService, ILogger<EmployeeTools> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    [McpServerTool(Name = "get_all_employees")]
    [Description("Gets all employees from the employee management system.")]
    [RequiredRoles("Employee", "Manager", "SuperAdmin")]
    public async Task<List<EmployeeDto>> GetAllEmployees()
    {
        try
        {
            return await _employeeService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get_all_employees failed");
            throw new McpException("Unable to retrieve employees at this time.");
        }
    }

    [McpServerTool(Name = "get_employee_by_id")]
    [Description("Gets an employee by their unique employee ID. Returns null if no employee with that ID exists.")]
    [RequiredRoles("Employee", "Manager", "SuperAdmin")]
    public async Task<EmployeeDto?> GetEmployeeById(
        [Description("The unique numeric ID of the employee")] int id)
    {
        if (id <= 0)
        {
            throw new McpException("Employee ID must be a positive integer.");
        }

        try
        {
            return await _employeeService.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get_employee_by_id failed for id {Id}", id);
            throw new McpException("Unable to retrieve the employee at this time.");
        }
    }

    [McpServerTool(Name = "create_employee")]
    [Description("Creates a new employee with the specified name.")]
    [RequiredRoles("Manager", "SuperAdmin")]
    public async Task<EmployeeDto> CreateEmployee(
        [Description("The full name of the employee. Required, maximum 100 characters.")] string name)
    {
        var dto = new CreateEmployeeDto { Name = name };
        DtoValidator.ValidateOrThrow(dto);

        try
        {
            return await _employeeService.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "create_employee failed for name {Name}", name);
            throw new McpException("Unable to create the employee. Verify the name is valid.");
        }
    }

    [McpServerTool(Name = "delete_employee")]
    [Description("Deletes an employee by ID. Returns whether an employee was actually deleted.")]
    [RequiredRoles("SuperAdmin")]
    public async Task<EmployeeDeleteResult> DeleteEmployee(
        [Description("The unique numeric ID of the employee to delete")] int id)
    {
        if (id <= 0)
        {
            throw new McpException("Employee ID must be a positive integer.");
        }

        try
        {
            var deleted = await _employeeService.DeleteAsync(id);
            return new EmployeeDeleteResult(deleted, deleted
                ? $"Employee {id} was deleted."
                : $"Employee {id} was not found; nothing was deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "delete_employee failed for id {Id}", id);
            throw new McpException("Unable to delete the employee at this time.");
        }
    }
}

public record EmployeeDeleteResult(bool Deleted, string Message);
