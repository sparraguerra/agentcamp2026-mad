namespace DndCopilot.Core.Entities;

public class LootTable
{
    public int Id { get; set; }
    public int NpcId { get; set; }
    public Npc Npc { get; set; } = null!;
    
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public double DropChance { get; set; } // 0.0 to 1.0
    public int MinQuantity { get; set; } = 1;
    public int MaxQuantity { get; set; } = 1;
}
