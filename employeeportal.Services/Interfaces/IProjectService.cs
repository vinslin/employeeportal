using employeeportal.Entity.DTOs;

namespace employeeportal.Services.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<bool> DeleteAsync(int id);
}
