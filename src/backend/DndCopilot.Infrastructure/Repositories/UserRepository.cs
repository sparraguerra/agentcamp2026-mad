using Microsoft.EntityFrameworkCore;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Interfaces;
using DndCopilot.Infrastructure.Data;

namespace DndCopilot.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(GameDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.Characters)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Characters)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
