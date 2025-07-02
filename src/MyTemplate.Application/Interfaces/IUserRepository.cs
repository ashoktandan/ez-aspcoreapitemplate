using MyTemplate.Domain.Entities;

namespace MyTemplate.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid id);
}
