using employeeportal.Data.Interfaces;
using employeeportal.Entity.DTOs;
using employeeportal.Entity.Entities;
using employeeportal.Services.Interfaces;

namespace employeeportal.Services.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllWithEmployeesAsync();
        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdWithEmployeesAsync(id);
        return project is null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Summary = dto.Summary,
            Client = dto.Client
        };

        var created = await _projectRepository.CreateAsync(project);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _projectRepository.DeleteAsync(id);
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Summary = project.Summary,
            Client = project.Client,
            Employees = project.EmployeeProjects
                .Select(ep => new EmployeeSummaryDto
                {
                    Id = ep.Employee.Id,
                    Name = ep.Employee.Name
                })
                .ToList()
        };
    }
}
