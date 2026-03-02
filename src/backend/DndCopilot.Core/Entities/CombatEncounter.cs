namespace DndCopilot.Core.Entities;

public class CombatEncounter
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    
    public int NpcId { get; set; }
    public Npc Npc { get; set; } = null!;
    
    public int CharacterCurrentHp { get; set; }
    public int NpcCurrentHp { get; set; }
    public int CurrentTurn { get; set; }
    public bool IsCharacterTurn { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
