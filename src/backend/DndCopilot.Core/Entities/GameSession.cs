namespace DndCopilot.Core.Entities;

public class GameSession
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; }
    
    public string CurrentLocation { get; set; } = "Village Square";
    public string PreviousLocation { get; set; } = "Village Square";
    public string CurrentScenario { get; set; } = string.Empty;
    public string ConversationHistory { get; set; } = string.Empty; // JSON string of messages
    public string? CurrentConversationNpc { get; set; } // Tracks the NPC currently in conversation
}
