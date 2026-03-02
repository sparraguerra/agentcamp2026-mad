namespace DndCopilot.Api.Models;

public class CombatActionRequest
{
    public int CharacterId { get; set; }
    public int NpcId { get; set; }
    public string Action { get; set; } = "attack"; // attack, defend, flee
}

public class CombatResponse
{
    public string Message { get; set; } = string.Empty;
    public int CharacterHp { get; set; }
    public int NpcHp { get; set; }
    public int Damage { get; set; }
    public bool IsCharacterDefeated { get; set; }
    public bool IsNpcDefeated { get; set; }
    public bool NpcFled { get; set; }
    public string? DiceRoll { get; set; }
}
