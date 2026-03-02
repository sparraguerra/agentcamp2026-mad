using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DndCopilot.Api.Models;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Interfaces;
using DndCopilot.Core.Services;
using DndCopilot.Infrastructure.Data;

namespace DndCopilot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ICharacterRepository _characterRepository;
    private readonly GameDbContext _context;
    private readonly GameService _gameService;
    private readonly CombatService _combatService;

    public GameController(
        ICharacterRepository characterRepository,
        GameDbContext context,
        GameService gameService,
        CombatService combatService)
    {
        _characterRepository = characterRepository;
        _context = context;
        _gameService = gameService;
        _combatService = combatService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartGameResponse>> StartGame([FromBody] StartGameRequest request)
    {
        var character = await _characterRepository.GetByIdAsync(request.CharacterId);
        if (character == null)
        {
            return NotFound(new { message = "Character not found" });
        }

        var userId = GetUserId();
        if (character.UserId != userId)
        {
            return Forbid();
        }

        // End any active sessions for this character
        var activeSessions = _context.GameSessions
            .Where(s => s.CharacterId == request.CharacterId && s.IsActive)
            .ToList();
        
        foreach (var session in activeSessions)
        {
            session.IsActive = false;
            session.EndedAt = DateTime.UtcNow;
        }

        // Create new game session
        var newSession = new GameSession
        {
            CharacterId = request.CharacterId,
            StartedAt = DateTime.UtcNow,
            IsActive = true,
            CurrentLocation = "Village Square",
            PreviousLocation = "Village Square",
            ConversationHistory = JsonSerializer.Serialize(new List<GameMessage>())
        };

        _context.GameSessions.Add(newSession);
        await _context.SaveChangesAsync();

        var welcomeMessage = _gameService.GenerateWelcomeMessage(character);

        var initialActions = _gameService.GetAvailableActions(newSession.CurrentLocation);

        return Ok(new StartGameResponse
        {
            SessionId = newSession.Id,
            WelcomeMessage = welcomeMessage,
            CharacterName = character.Name,
            CharacterClass = character.Class.ToString(),
            AvailableActions = initialActions,
            GameState = new Dictionary<string, object>
            {
                ["location"] = newSession.CurrentLocation,
                ["characterHp"] = character.HitPoints,
                ["characterMaxHp"] = character.MaxHitPoints,
                ["gold"] = character.Gold,
                ["isInCombat"] = false
            }
        });
    }

    [HttpPost("action")]
    public async Task<ActionResult<GameActionResponse>> PerformAction([FromBody] GameActionRequest request)
    {
        var session = await _context.GameSessions
            .Include(s => s.Character)
                .ThenInclude(c => c.Inventory)
                .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.IsActive);

        if (session == null)
        {
            return NotFound(new { message = "Game session not found or inactive" });
        }

        var userId = GetUserId();
        if (session.Character.UserId != userId)
        {
            return Forbid();
        }

        var action = request.Action.ToLower().Trim();

        // Check if this action has sub-actions before processing
        if (_gameService.ActionHasSubActions(session.CurrentLocation, action))
        {
            var subActions = _gameService.GetActionSubActions(session.CurrentLocation, action);
            return Ok(new GameActionResponse
            {
                Response = $"Select an option:",
                AvailableActions = _gameService.GetAvailableActions(session.CurrentLocation),
                SubActions = subActions,
                SelectedParentAction = request.Action,
                GameState = new Dictionary<string, object>
                {
                    ["location"] = session.CurrentLocation,
                    ["characterHp"] = session.Character.HitPoints,
                    ["characterMaxHp"] = session.Character.MaxHitPoints,
                    ["gold"] = session.Character.Gold,
                    ["isInCombat"] = false
                }
            });
        }

        // Check for active combat encounter
        var activeCombat = await _context.CombatEncounters
            .Include(e => e.Npc)
            .FirstOrDefaultAsync(e => e.CharacterId == session.CharacterId && e.IsActive);

        string response;
        List<string> actions;

        if (activeCombat != null)
        {
            (response, actions) = HandleCombatAction(
                session.Character,
                activeCombat,
                action,
                request.PlayerRollTotal,
                request.PlayerRollNotation);
            // Prevent NPC template data from being overwritten
            _context.Entry(activeCombat.Npc).State = EntityState.Unchanged;
        }
        else if (action.Contains("fight") || action.Contains("attack") || action.Contains("combat"))
        {
            (response, actions) = await InitiateCombatEncounter(session);
        }
        else
        {
            (response, actions) = await _gameService.ProcessActionAsync(
                session,
                request.Action,
                request.PlayerRollTotal,
                request.PlayerRollNotation);
        }

        // Update conversation history
        var history = JsonSerializer.Deserialize<List<GameMessage>>(session.ConversationHistory) ?? new List<GameMessage>();
        history.Add(new GameMessage
        {
            Role = "user",
            Content = request.PlayerRollTotal.HasValue
                ? $"{request.Action} [roll {request.PlayerRollNotation ?? "1d20"}={request.PlayerRollTotal.Value}]"
                : request.Action,
            Timestamp = DateTime.UtcNow
        });
        history.Add(new GameMessage
        {
            Role = "assistant",
            Content = response,
            Timestamp = DateTime.UtcNow
        });
        session.ConversationHistory = JsonSerializer.Serialize(history);

        _context.GameSessions.Update(session);
        await _context.SaveChangesAsync();

        return Ok(new GameActionResponse
        {
            Response = response,
            AvailableActions = actions,
            GameState = new Dictionary<string, object>
            {
                ["location"] = session.CurrentLocation,
                ["characterHp"] = session.Character.HitPoints,
                ["characterMaxHp"] = session.Character.MaxHitPoints,
                ["gold"] = session.Character.Gold,
                ["isInCombat"] = activeCombat != null && activeCombat.IsActive
            }
        });
    }

    private async Task<(string response, List<string> actions)> InitiateCombatEncounter(GameSession session)
    {
        var hostileNpcs = await _context.Npcs.Where(n => n.IsHostile).ToListAsync();
        if (!hostileNpcs.Any())
        {
            return ("There are no enemies nearby.", new List<string> { "Look around", "Rest" });
        }

        var npc = hostileNpcs[Random.Shared.Next(hostileNpcs.Count)];

        var encounter = new CombatEncounter
        {
            CharacterId = session.CharacterId,
            NpcId = npc.Id,
            CharacterCurrentHp = session.Character.HitPoints,
            NpcCurrentHp = npc.MaxHitPoints,
            CurrentTurn = 0,
            IsCharacterTurn = true,
            IsActive = true,
            StartedAt = DateTime.UtcNow
        };

        _context.CombatEncounters.Add(encounter);

        var response = $@"⚔️ COMBAT INITIATED!

A {npc.Name.ToLower()} jumps out from the shadows!

🧌 {npc.Name}
HP: {npc.MaxHitPoints}/{npc.MaxHitPoints}
ATK: {npc.AttackPower}

Prepare for battle!";

        var actions = new List<string> { "Attack", "Defend", "Use item", "Flee" };
        return (response, actions);
    }

    private (string response, List<string> actions) HandleCombatAction(
        Character character,
        CombatEncounter encounter,
        string action,
        int? playerRollTotal,
        string? playerRollNotation)
    {
        if (!playerRollTotal.HasValue)
        {
            var rollRequiredResponse = $@"🎲 COMBAT ROLL REQUIRED

You are fighting {encounter.Npc.Name}.
Roll your dice before choosing a combat action.

Recommended: 1d20";

            return (rollRequiredResponse, new List<string> { "Attack", "Defend", "Flee" });
        }

        if (action.Contains("attack"))
        {
            return ExecuteCombatAttack(character, encounter, playerRollTotal.Value, playerRollNotation);
        }
        else if (action.Contains("defend"))
        {
            return ExecuteCombatDefend(character, encounter, playerRollTotal.Value, playerRollNotation);
        }
        else if (action.Contains("flee") || action.Contains("run"))
        {
            return ExecuteCombatFlee(character, encounter, playerRollTotal.Value, playerRollNotation);
        }
        else
        {
            var response = $@"⚔️ You're in the middle of combat with {encounter.Npc.Name}!

🧌 {encounter.Npc.Name} HP: {encounter.NpcCurrentHp}/{encounter.Npc.MaxHitPoints}
❤️ Your HP: {encounter.CharacterCurrentHp}/{character.MaxHitPoints}

Choose your action!";
            return (response, new List<string> { "Attack", "Defend", "Flee" });
        }
    }

    private (string response, List<string> actions) ExecuteCombatAttack(
        Character character,
        CombatEncounter encounter,
        int playerRollTotal,
        string? playerRollNotation)
    {
        var npc = encounter.Npc;
        // Set NPC HP to the encounter-tracked value
        npc.HitPoints = encounter.NpcCurrentHp;

        var sb = new StringBuilder();
        sb.AppendLine($"🎲 Your roll: {playerRollNotation ?? "1d20"} = {playerRollTotal}");

        if (playerRollTotal < 12)
        {
            sb.AppendLine("⚠️ You miss your timing and fail to land a solid attack.");
            sb.AppendLine();

            var npcResultOnMiss = _combatService.ExecuteTurn(character, npc, false);
            sb.AppendLine($"🧌 {npcResultOnMiss.Message}");
            sb.AppendLine($"💥 Damage to you: {npcResultOnMiss.ActualDamage}");
            sb.AppendLine($"❤️ Your HP: {character.HitPoints}/{character.MaxHitPoints}");

            if (npcResultOnMiss.IsDefenderDefeated)
            {
                return FinishCombatDefeat(character, encounter, sb);
            }

            encounter.NpcCurrentHp = npc.HitPoints;
            encounter.CharacterCurrentHp = character.HitPoints;
            encounter.CurrentTurn++;
            return (sb.ToString(), new List<string> { "Attack", "Defend", "Flee" });
        }

        // Player attacks
        var playerResult = _combatService.ExecuteTurn(character, npc, true);
        sb.AppendLine($"⚔️ {playerResult.Message}");
        sb.AppendLine($"🎲 Attack Roll: {playerResult.AttackRoll}");
        sb.AppendLine($"💥 Damage dealt: {playerResult.ActualDamage}");
        sb.AppendLine($"🧌 {npc.Name} HP: {npc.HitPoints}/{npc.MaxHitPoints}");

        if (playerResult.IsDefenderDefeated)
        {
            return FinishCombatVictory(character, encounter, npc, sb);
        }

        // NPC counter-attacks
        sb.AppendLine();
        var npcResult = _combatService.ExecuteTurn(character, npc, false);
        sb.AppendLine($"🧌 {npcResult.Message}");
        sb.AppendLine($"💥 Damage to you: {npcResult.ActualDamage}");
        sb.AppendLine($"❤️ Your HP: {character.HitPoints}/{character.MaxHitPoints}");

        if (npcResult.IsDefenderDefeated)
        {
            return FinishCombatDefeat(character, encounter, sb);
        }

        // Update encounter state
        encounter.NpcCurrentHp = npc.HitPoints;
        encounter.CharacterCurrentHp = character.HitPoints;
        encounter.CurrentTurn++;

        return (sb.ToString(), new List<string> { "Attack", "Defend", "Flee" });
    }

    private (string response, List<string> actions) ExecuteCombatDefend(
        Character character,
        CombatEncounter encounter,
        int playerRollTotal,
        string? playerRollNotation)
    {
        var npc = encounter.Npc;
        npc.HitPoints = encounter.NpcCurrentHp;

        var sb = new StringBuilder();
        sb.AppendLine($"🎲 Your roll: {playerRollNotation ?? "1d20"} = {playerRollTotal}");
        sb.AppendLine("🛡️ You brace yourself and raise your guard!");
        sb.AppendLine();

        // NPC attacks, but damage is halved
        var npcResult = _combatService.ExecuteTurn(character, npc, false);
        // Good roll improves defense, low roll weakens it.
        int mitigated = playerRollTotal >= 10 ? npcResult.ActualDamage / 2 : npcResult.ActualDamage / 4;
        character.HitPoints = Math.Min(character.MaxHitPoints, character.HitPoints + mitigated);

        sb.AppendLine($"🧌 {npcResult.Message}");
        sb.AppendLine($"🛡️ Your defense mitigated {mitigated} damage!");
        sb.AppendLine($"❤️ Your HP: {character.HitPoints}/{character.MaxHitPoints}");

        if (character.HitPoints <= 0)
        {
            return FinishCombatDefeat(character, encounter, sb);
        }

        encounter.NpcCurrentHp = npc.HitPoints;
        encounter.CharacterCurrentHp = character.HitPoints;
        encounter.CurrentTurn++;

        return (sb.ToString(), new List<string> { "Attack", "Defend", "Flee" });
    }

    private (string response, List<string> actions) ExecuteCombatFlee(
        Character character,
        CombatEncounter encounter,
        int playerRollTotal,
        string? playerRollNotation)
    {
        var npc = encounter.Npc;
        npc.HitPoints = encounter.NpcCurrentHp;

        // Flee depends on player's roll + dexterity bonus.
        int fleeScore = playerRollTotal + ((character.Dexterity - 10) / 2);
        bool escaped = fleeScore >= 12;

        var sb = new StringBuilder();
        sb.AppendLine($"🎲 Your roll: {playerRollNotation ?? "1d20"} = {playerRollTotal}");

        if (escaped)
        {
            encounter.IsActive = false;
            encounter.EndedAt = DateTime.UtcNow;

            sb.AppendLine("🏃 You successfully flee from combat!");
            sb.AppendLine($"You escape the {npc.Name} and catch your breath.");
            return (sb.ToString(), new List<string> { "Look around", "Rest", "Check inventory" });
        }

        sb.AppendLine("🏃 You try to flee but the enemy blocks your path!");
        sb.AppendLine();

        // NPC gets a free attack
        var npcResult = _combatService.ExecuteTurn(character, npc, false);
        sb.AppendLine($"🧌 {npcResult.Message}");
        sb.AppendLine($"❤️ Your HP: {character.HitPoints}/{character.MaxHitPoints}");

        if (npcResult.IsDefenderDefeated)
        {
            return FinishCombatDefeat(character, encounter, sb);
        }

        encounter.NpcCurrentHp = npc.HitPoints;
        encounter.CharacterCurrentHp = character.HitPoints;
        encounter.CurrentTurn++;

        return (sb.ToString(), new List<string> { "Attack", "Defend", "Flee" });
    }

    private (string response, List<string> actions) FinishCombatVictory(Character character, CombatEncounter encounter, Npc npc, StringBuilder sb)
    {
        encounter.IsActive = false;
        encounter.EndedAt = DateTime.UtcNow;
        encounter.NpcCurrentHp = 0;

        var xpGain = npc.ExperienceReward;
        var goldGain = npc.GoldReward;
        character.Experience += xpGain;
        character.Gold += goldGain;

        sb.AppendLine();
        sb.AppendLine($"🎉 VICTORY! You defeated the {npc.Name}!");
        sb.AppendLine($"⭐ Experience: +{xpGain} XP");
        sb.AppendLine($"💰 Gold: +{goldGain}");

        // Check level up (100 XP per level)
        if (character.Experience >= character.Level * 100)
        {
            character.Level++;
            character.MaxHitPoints += 20;
            character.HitPoints = character.MaxHitPoints;
            character.Strength += 2;
            character.Dexterity += 1;
            character.Constitution += 1;

            sb.AppendLine();
            sb.AppendLine($"🆙 LEVEL UP! You are now level {character.Level}!");
            sb.AppendLine($"❤️ Max HP: {character.MaxHitPoints}");
            sb.AppendLine($"💪 STR: {character.Strength}");
        }

        return (sb.ToString(), new List<string> { "Look around", "Check inventory", "Rest" });
    }

    private (string response, List<string> actions) FinishCombatDefeat(Character character, CombatEncounter encounter, StringBuilder sb)
    {
        encounter.IsActive = false;
        encounter.EndedAt = DateTime.UtcNow;
        character.HitPoints = 1; // Survive with 1 HP

        sb.AppendLine();
        sb.AppendLine("💀 You have been defeated! You barely escape with your life...");
        sb.AppendLine("❤️ HP: 1/" + character.MaxHitPoints);

        return (sb.ToString(), new List<string> { "Rest", "Go to tavern" });
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<object>> GetSession(int sessionId)
    {
        var session = await _context.GameSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null)
        {
            return NotFound(new { message = "Session not found" });
        }

        var userId = GetUserId();
        if (session.Character.UserId != userId)
        {
            return Forbid();
        }

        var history = JsonSerializer.Deserialize<List<GameMessage>>(session.ConversationHistory) ?? new List<GameMessage>();

        return Ok(new
        {
            sessionId = session.Id,
            characterName = session.Character.Name,
            isActive = session.IsActive,
            currentLocation = session.CurrentLocation,
            conversationHistory = history,
            startedAt = session.StartedAt
        });
    }

    [HttpPost("end/{sessionId}")]
    public async Task<ActionResult> EndSession(int sessionId)
    {
        var session = await _context.GameSessions
            .Include(s => s.Character)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.IsActive);

        if (session == null)
        {
            return NotFound(new { message = "Active session not found" });
        }

        var userId = GetUserId();
        if (session.Character.UserId != userId)
        {
            return Forbid();
        }

        session.IsActive = false;
        session.EndedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Game session ended" });
    }
}
