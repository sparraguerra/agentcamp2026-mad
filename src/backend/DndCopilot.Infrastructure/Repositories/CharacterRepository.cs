using Microsoft.EntityFrameworkCore;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Interfaces;
using DndCopilot.Infrastructure.Data;

namespace DndCopilot.Infrastructure.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(GameDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Character>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Character?> GetWithInventoryAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Inventory)
            .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Character?> GetWithQuestsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Quests)
            .ThenInclude(cq => cq.Quest)
            .ThenInclude(q => q.Stages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
