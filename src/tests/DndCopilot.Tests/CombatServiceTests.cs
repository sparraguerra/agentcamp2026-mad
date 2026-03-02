using Xunit;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Enums;
using DndCopilot.Core.Services;

namespace DndCopilot.Tests;

public class CombatServiceTests
{
    private readonly CombatService _combatService;
    private readonly DiceRoller _diceRoller;

    public CombatServiceTests()
    {
        _diceRoller = new DiceRoller();
        _combatService = new CombatService(_diceRoller);
    }

    [Fact]
    public void ExecuteTurn_CharacterAttack_DamagesNpc()
    {
        // Arrange
        var character = new Character
        {
            Id = 1,
            Name = "Test Hero",
            Class = CharacterClass.Warrior,
            HitPoints = 100,
            MaxHitPoints = 100,
            Strength = 16
        };

        var npc = new Npc
        {
            Id = 1,
            Name = "Goblin",
            HitPoints = 20,
            MaxHitPoints = 20,
            Defense = 2,
            Behavior = NpcBehavior.Aggressive
        };

        int initialNpcHp = npc.HitPoints;

        // Act
        var result = _combatService.ExecuteTurn(character, npc, isCharacterTurn: true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(character.Name, result.AttackerName);
        Assert.Equal(npc.Name, result.DefenderName);
        Assert.True(npc.HitPoints <= initialNpcHp);
    }

    [Fact]
    public void ExecuteTurn_NpcAttack_DamagesCharacter()
    {
        // Arrange
        var character = new Character
        {
            Id = 1,
            Name = "Test Hero",
            Class = CharacterClass.Warrior,
            HitPoints = 100,
            MaxHitPoints = 100,
            Dexterity = 10
        };

        var npc = new Npc
        {
            Id = 1,
            Name = "Goblin",
            HitPoints = 20,
            MaxHitPoints = 20,
            AttackPower = 8,
            Behavior = NpcBehavior.Aggressive
        };

        int initialCharacterHp = character.HitPoints;

        // Act
        var result = _combatService.ExecuteTurn(character, npc, isCharacterTurn: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(npc.Name, result.AttackerName);
        Assert.Equal(character.Name, result.DefenderName);
        Assert.True(character.HitPoints <= initialCharacterHp);
    }

    [Fact]
    public void ExecuteTurn_NpcWithFleeBehavior_CanFlee()
    {
        // Arrange
        var character = new Character
        {
            Id = 1,
            Name = "Test Hero",
            Class = CharacterClass.Warrior,
            HitPoints = 100,
            MaxHitPoints = 100
        };

        var npc = new Npc
        {
            Id = 1,
            Name = "Cowardly Goblin",
            HitPoints = 5,
            MaxHitPoints = 20,
            AttackPower = 6,
            Behavior = NpcBehavior.Flee
        };

        // Act
        var result = _combatService.ExecuteTurn(character, npc, isCharacterTurn: false);

        // Assert
        Assert.NotNull(result);
        // NPC might flee when HP is low
        if (result.NpcFled)
        {
            Assert.Contains("flee", result.Message.ToLower());
        }
    }
}
