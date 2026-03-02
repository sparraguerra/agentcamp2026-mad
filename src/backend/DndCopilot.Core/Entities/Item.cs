using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Entities;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public bool IsStackable { get; set; }
    public bool IsEquippable { get; set; }
    public int Value { get; set; }
    public int? AttackBonus { get; set; }
    public int? DefenseBonus { get; set; }
    public int? HealthRestore { get; set; }
}
