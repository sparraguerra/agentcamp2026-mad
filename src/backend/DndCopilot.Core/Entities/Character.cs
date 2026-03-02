using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Entities;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int HitPoints { get; set; } = 100;
    public int MaxHitPoints { get; set; } = 100;
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;
    public int Gold { get; set; } = 0;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public List<InventoryItem> Inventory { get; set; } = new();
    public List<CharacterQuest> Quests { get; set; } = new();
}
