using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DndCopilot.Api.Models;
using DndCopilot.Core.Entities;
using DndCopilot.Core.Interfaces;

namespace DndCopilot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IUserRepository _userRepository;

    public CharactersController(ICharacterRepository characterRepository, IUserRepository userRepository)
    {
        _characterRepository = characterRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponse>>> GetCharacters()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Missing or invalid user id.");

        var characters = await _characterRepository.GetByUserIdAsync(userId);

        return Ok(characters.Select(c => new CharacterResponse
        {
            Id = c.Id,
            Name = c.Name,
            Class = c.Class,
            Level = c.Level,
            Experience = c.Experience,
            HitPoints = c.HitPoints,
            MaxHitPoints = c.MaxHitPoints,
            Strength = c.Strength,
            Dexterity = c.Dexterity,
            Constitution = c.Constitution,
            Intelligence = c.Intelligence,
            Wisdom = c.Wisdom,
            Charisma = c.Charisma,
            Gold = c.Gold
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterResponse>> GetCharacter(int id)
    {
        var character = await _characterRepository.GetByIdAsync(id);
        if (character == null)
            return NotFound();

        if (!TryGetUserId(out var userId))
            return Unauthorized("Missing or invalid user id.");

        if (character.UserId != userId)
            return Forbid();

        return Ok(new CharacterResponse
        {
            Id = character.Id,
            Name = character.Name,
            Class = character.Class,
            Level = character.Level,
            Experience = character.Experience,
            HitPoints = character.HitPoints,
            MaxHitPoints = character.MaxHitPoints,
            Strength = character.Strength,
            Dexterity = character.Dexterity,
            Constitution = character.Constitution,
            Intelligence = character.Intelligence,
            Wisdom = character.Wisdom,
            Charisma = character.Charisma,
            Gold = character.Gold
        });
    }

    [HttpPost]
    public async Task<ActionResult<CharacterResponse>> CreateCharacter([FromBody] CreateCharacterRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Missing or invalid user id.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Unauthorized("User does not exist.");

        var character = new Character
        {
            Name = request.Name,
            Class = request.Class,
            UserId = userId,
            Level = 1,
            Experience = 0,
            HitPoints = 100,
            MaxHitPoints = 100,
            Strength = 10,
            Dexterity = 10,
            Constitution = 10,
            Intelligence = 10,
            Wisdom = 10,
            Charisma = 10,
            Gold = 100
        };

        await _characterRepository.AddAsync(character);

        return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, new CharacterResponse
        {
            Id = character.Id,
            Name = character.Name,
            Class = character.Class,
            Level = character.Level,
            Experience = character.Experience,
            HitPoints = character.HitPoints,
            MaxHitPoints = character.MaxHitPoints,
            Strength = character.Strength,
            Dexterity = character.Dexterity,
            Constitution = character.Constitution,
            Intelligence = character.Intelligence,
            Wisdom = character.Wisdom,
            Charisma = character.Charisma,
            Gold = character.Gold
        });
    }

    private bool TryGetUserId(out int userId)
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out userId) && userId > 0;
    }
}
