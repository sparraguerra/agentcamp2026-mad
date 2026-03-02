using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DndCopilot.Api.Models;
using DndCopilot.Core.Interfaces;
using DndCopilot.Core.Services;
using DndCopilot.Infrastructure.Data;

namespace DndCopilot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CombatController : ControllerBase
{
    private readonly ICharacterRepository _characterRepository;
    private readonly GameDbContext _context;
    private readonly CombatService _combatService;

    public CombatController(
        ICharacterRepository characterRepository, 
        GameDbContext context,
        CombatService combatService)
    {
        _characterRepository = characterRepository;
        _context = context;
        _combatService = combatService;
    }

    [HttpPost("attack")]
    public async Task<ActionResult<CombatResponse>> Attack([FromBody] CombatActionRequest request)
    {
        var character = await _characterRepository.GetByIdAsync(request.CharacterId);
        if (character == null)
        {
            return NotFound(new { message = "Character not found" });
        }

        var npc = await _context.Npcs.FindAsync(request.NpcId);
        if (npc == null)
        {
            return NotFound(new { message = "NPC not found" });
        }

        var result = _combatService.ExecuteTurn(character, npc, true);

        // Update character and npc in database
        await _characterRepository.UpdateAsync(character);
        _context.Npcs.Update(npc);
        await _context.SaveChangesAsync();

        return Ok(new CombatResponse
        {
            Message = result.Message,
            CharacterHp = character.HitPoints,
            NpcHp = npc.HitPoints,
            Damage = result.ActualDamage,
            IsCharacterDefeated = character.HitPoints <= 0,
            IsNpcDefeated = npc.HitPoints <= 0,
            NpcFled = result.NpcFled,
            DiceRoll = result.AttackRoll?.ToString()
        });
    }
}
