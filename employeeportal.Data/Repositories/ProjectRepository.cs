using employeeportal.Data.Interfaces;
using employeeportal.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace employeeportal.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllWithEmployeesAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdWithEmployeesAsync(int id)
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return false;
        }

        var links = _context.EmployeeProjects.Where(ep => ep.ProjectId == id);
        _context.EmployeeProjects.RemoveRange(links);
        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Projects.AnyAsync(p => p.Id == id);
    }
}
