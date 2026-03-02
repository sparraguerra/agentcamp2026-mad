using DndCopilot.Core.Enums;

namespace DndCopilot.Api.Models;

public class CreateCharacterRequest
{
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
}

public class CharacterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }
    public int Gold { get; set; }
}
