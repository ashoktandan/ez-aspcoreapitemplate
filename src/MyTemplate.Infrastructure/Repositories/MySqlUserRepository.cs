using MyTemplate.Application.Interfaces;
using MyTemplate.Domain.Entities;

namespace MyTemplate.Infrastructure.Repositories;

#if mysql
public class MySqlUserRepository : IUserRepository
{
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        // Dummy MySQL Implementation
        return Task.FromResult(new User { Id = id, Name = "MySQL User" });
    }
}
#endif
