using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Entities;

public class CharacterQuest
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    
    public int QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public QuestStatus Status { get; set; }
    public int CurrentStage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
