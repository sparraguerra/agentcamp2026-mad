namespace DndCopilot.Core.Entities;

public class QuestReward
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public int Quantity { get; set; } = 1;
}
