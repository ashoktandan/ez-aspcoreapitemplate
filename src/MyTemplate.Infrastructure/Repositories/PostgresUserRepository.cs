using MyTemplate.Application.Interfaces;
using MyTemplate.Domain.Entities;

namespace MyTemplate.Infrastructure.Repositories;

#if postgres
public class PostgresUserRepository : IUserRepository
{
    public Task<User?> GetUserByIdAsync(Guid id)
    {
        // Dummy PostgreSQL Implementation
        return Task.FromResult(new User { Id = id, Name = "Postgres User" });
    }
}
#endif
