using employeeportal.Data.Interfaces;
using employeeportal.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace employeeportal.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees.AsNoTracking().ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee is null)
        {
            return false;
        }

        // EmployeeProject has no ON DELETE CASCADE on FK_EmployeeProject_Employee,
        // so any project assignments must be removed first or SQL Server rejects
        // the delete with a foreign key violation.
        var links = _context.EmployeeProjects.Where(ep => ep.EmployeeId == id);
        _context.EmployeeProjects.RemoveRange(links);
        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();
        return true;
    }
}
