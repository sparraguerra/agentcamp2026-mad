namespace DndCopilot.Core.Entities;

public class QuestStage
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CompletionCriteria { get; set; } = string.Empty;
}
