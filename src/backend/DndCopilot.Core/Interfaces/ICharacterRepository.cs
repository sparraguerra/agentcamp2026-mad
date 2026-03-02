using DndCopilot.Core.Entities;

namespace DndCopilot.Core.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<IEnumerable<Character>> GetByUserIdAsync(int userId);
    Task<Character?> GetWithInventoryAsync(int id);
    Task<Character?> GetWithQuestsAsync(int id);
}
