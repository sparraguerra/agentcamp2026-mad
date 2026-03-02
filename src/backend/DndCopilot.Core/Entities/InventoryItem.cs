namespace DndCopilot.Core.Entities;

public class InventoryItem
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public int Quantity { get; set; } = 1;
    public bool IsEquipped { get; set; }
}
