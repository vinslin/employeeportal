using System.ComponentModel;
using employeeportal.Entity.DTOs;
using employeeportal.Mcp.Authorization;
using employeeportal.Services.Interfaces;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace employeeportal.Mcp.Tools;

[McpServerToolType]
public class ProjectTools
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectTools> _logger;

    public ProjectTools(IProjectService projectService, ILogger<ProjectTools> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    [McpServerTool(Name = "get_all_projects_with_employees")]
    [Description("Gets all projects and the employees assigned to each project.")]
    [RequiredRoles("Employee", "Manager", "SuperAdmin")]
    public async Task<List<ProjectDto>> GetAllProjectsWithEmployees()
    {
        try
        {
            return await _projectService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get_all_projects_with_employees failed");
            throw new McpException("Unable to retrieve projects at this time.");
        }
    }

    [McpServerTool(Name = "get_project_by_id")]
    [Description("Gets a project by ID including its assigned employees. Returns null if no project with that ID exists.")]
    [RequiredRoles("Employee", "Manager", "SuperAdmin")]
    public async Task<ProjectDto?> GetProjectById(
        [Description("The unique numeric ID of the project")] int id)
    {
        if (id <= 0)
        {
            throw new McpException("Project ID must be a positive integer.");
        }

        try
        {
            return await _projectService.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get_project_by_id failed for id {Id}", id);
            throw new McpException("Unable to retrieve the project at this time.");
        }
    }

    [McpServerTool(Name = "create_project")]
    [Description("Creates a new project.")]
    [RequiredRoles("Manager", "SuperAdmin")]
    public async Task<ProjectDto> CreateProject(
        [Description("The name of the project. Maximum 200 characters.")] string name,
        [Description("The client the project is being built for. Maximum 200 characters.")] string client,
        [Description("A short summary of the project. Optional, maximum 500 characters.")] string? summary = null)
    {
        var dto = new CreateProjectDto { Name = name, Client = client, Summary = summary };
        DtoValidator.ValidateOrThrow(dto);

        try
        {
            return await _projectService.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "create_project failed for name {Name}", name);
            throw new McpException("Unable to create the project. Verify the input values.");
        }
    }

    [McpServerTool(Name = "delete_project")]
    [Description("Deletes a project by ID without deleting the employees assigned to it. Returns whether a project was actually deleted.")]
    [RequiredRoles("SuperAdmin")]
    public async Task<ProjectDeleteResult> DeleteProject(
        [Description("The unique numeric ID of the project to delete")] int id)
    {
        if (id <= 0)
        {
            throw new McpException("Project ID must be a positive integer.");
        }

        try
        {
            var deleted = await _projectService.DeleteAsync(id);
            return new ProjectDeleteResult(deleted, deleted
                ? $"Project {id} was deleted."
                : $"Project {id} was not found; nothing was deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "delete_project failed for id {Id}", id);
            throw new McpException("Unable to delete the project at this time.");
        }
    }
}

public record ProjectDeleteResult(bool Deleted, string Message);
