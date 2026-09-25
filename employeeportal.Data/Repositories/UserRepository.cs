using employeeportal.Data.Interfaces;
using employeeportal.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace employeeportal.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserNameWithRolesAsync(string userName)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
