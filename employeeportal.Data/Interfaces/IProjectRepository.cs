using employeeportal.Entity.Entities;

namespace employeeportal.Data.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllWithEmployeesAsync();
    Task<Project?> GetByIdWithEmployeesAsync(int id);
    Task<Project> CreateAsync(Project project);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
