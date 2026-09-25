using employeeportal.Entity.Entities;

namespace employeeportal.Data.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameWithRolesAsync(string userName);
}
