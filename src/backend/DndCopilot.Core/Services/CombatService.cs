using DndCopilot.Core.Entities;
using DndCopilot.Core.Enums;

namespace DndCopilot.Core.Services;

public class CombatService
{
    private readonly DiceRoller _diceRoller;

    public CombatService(DiceRoller diceRoller)
    {
        _diceRoller = diceRoller;
    }

    public CombatResult ExecuteTurn(Character character, Npc npc, bool isCharacterTurn)
    {
        if (isCharacterTurn)
        {
            return CharacterAttack(character, npc);
        }
        else
        {
            return NpcAttack(character, npc);
        }
    }

    private CombatResult CharacterAttack(Character character, Npc npc)
    {
        // Roll attack (1d20 + strength modifier)
        var attackRoll = _diceRoller.Roll(1, 20, GetModifier(character.Strength));
        
        // Roll damage based on character class
        var damageRoll = GetCharacterDamage(character);
        
        int damage = Math.Max(0, damageRoll.Total - npc.Defense);
        npc.HitPoints = Math.Max(0, npc.HitPoints - damage);

        return new CombatResult
        {
            AttackerName = character.Name,
            DefenderName = npc.Name,
            AttackRoll = attackRoll,
            DamageRoll = damageRoll,
            ActualDamage = damage,
            DefenderHpRemaining = npc.HitPoints,
            IsDefenderDefeated = npc.HitPoints <= 0,
            Message = $"{character.Name} attacks {npc.Name} for {damage} damage!"
        };
    }

    private CombatResult NpcAttack(Character character, Npc npc)
    {
        // Determine NPC action based on behavior
        var action = DetermineNpcAction(npc, character);
        
        if (action == "flee")
        {
            return new CombatResult
            {
                AttackerName = npc.Name,
                DefenderName = character.Name,
                Message = $"{npc.Name} attempts to flee from combat!",
                NpcFled = true
            };
        }

        // Roll attack
        var attackRoll = _diceRoller.Roll(1, 20);
        
        // Roll damage
        var damageRoll = _diceRoller.Roll(1, npc.AttackPower);
        
        int damage = action == "defensive" 
            ? Math.Max(0, damageRoll.Total / 2 - GetDefense(character))
            : Math.Max(0, damageRoll.Total - GetDefense(character));
        
        character.HitPoints = Math.Max(0, character.HitPoints - damage);

        return new CombatResult
        {
            AttackerName = npc.Name,
            DefenderName = character.Name,
            AttackRoll = attackRoll,
            DamageRoll = damageRoll,
            ActualDamage = damage,
            DefenderHpRemaining = character.HitPoints,
            IsDefenderDefeated = character.HitPoints <= 0,
            Message = $"{npc.Name} attacks {character.Name} for {damage} damage!"
        };
    }

    private string DetermineNpcAction(Npc npc, Character character)
    {
        double hpPercentage = (double)npc.HitPoints / npc.MaxHitPoints;
        
        return npc.Behavior switch
        {
            NpcBehavior.Aggressive => "attack",
            NpcBehavior.Defensive => "defensive",
            NpcBehavior.Flee => hpPercentage < 0.3 ? "flee" : "attack",
            _ => "attack"
        };
    }

    private DiceResult GetCharacterDamage(Character character)
    {
        // Base damage by class
        return character.Class switch
        {
            CharacterClass.Warrior => _diceRoller.Roll(1, 10, GetModifier(character.Strength)),
            CharacterClass.Rogue => _diceRoller.Roll(1, 8, GetModifier(character.Dexterity)),
            CharacterClass.Mage => _diceRoller.Roll(1, 6, GetModifier(character.Intelligence)),
            CharacterClass.Cleric => _diceRoller.Roll(1, 8, GetModifier(character.Wisdom)),
            CharacterClass.Ranger => _diceRoller.Roll(1, 8, GetModifier(character.Dexterity)),
            _ => _diceRoller.Roll(1, 6)
        };
    }

    private int GetDefense(Character character)
    {
        return GetModifier(character.Dexterity);
    }

    private int GetModifier(int abilityScore)
    {
        return (abilityScore - 10) / 2;
    }
}

public class CombatResult
{
    public string AttackerName { get; set; } = string.Empty;
    public string DefenderName { get; set; } = string.Empty;
    public DiceResult? AttackRoll { get; set; }
    public DiceResult? DamageRoll { get; set; }
    public int ActualDamage { get; set; }
    public int DefenderHpRemaining { get; set; }
    public bool IsDefenderDefeated { get; set; }
    public bool NpcFled { get; set; }
    public string Message { get; set; } = string.Empty;
}
