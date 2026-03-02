using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Entities;

public class Npc
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public NpcBehavior Behavior { get; set; }
    public bool IsHostile { get; set; }
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    
    public List<LootTable> LootTables { get; set; } = new();
}
