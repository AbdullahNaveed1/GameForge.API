using System.Security.Claims;
using GameForge.API.Data;
using GameForge.API.DTOs;
using GameForge.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly AppDbContext _context;
    private const int MaxCharactersPerPlayer = 4;
    private static readonly HashSet<string> AllowedClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Warrior", "Mage", "Rogue", "Archer"
    };

    public CharactersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/characters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponseDto>>> GetMyCharacters()
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var characters = await _context.Characters
            .Where(c => c.PlayerId == playerId.Value)
            .Select(c => new CharacterResponseDto(
                c.Id,
                c.Name,
                c.CharacterClass,
                c.Level,
                c.Experience,
                c.Health,
                c.Mana,
                c.CreatedAt
            ))
            .ToListAsync();

        return Ok(characters);
    }

    // GET: api/characters/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterResponseDto>> GetCharacterById(Guid id)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId.Value);

        if (character == null) return NotFound("Character not found.");

        return Ok(new CharacterResponseDto(
            character.Id,
            character.Name,
            character.CharacterClass,
            character.Level,
            character.Experience,
            character.Health,
            character.Mana,
            character.CreatedAt
        ));
    }

    // POST: api/characters
    [HttpPost]
    public async Task<ActionResult<CharacterResponseDto>> CreateCharacter(CreateCharacterDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        // Rule 1: Validate Character Class
        if (!AllowedClasses.Contains(dto.CharacterClass))
        {
            return BadRequest($"Invalid class. Allowed classes are: {string.Join(", ", AllowedClasses)}");
        }

        // Rule 2: Enforce Slot Limit (Max 4 per player)
        var characterCount = await _context.Characters.CountAsync(c => c.PlayerId == playerId.Value);
        if (characterCount >= MaxCharactersPerPlayer)
        {
            return BadRequest($"Character limit reached ({MaxCharactersPerPlayer} max per account).");
        }

        // Rule 3: Enforce Unique Name
        var nameExists = await _context.Characters.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
        if (nameExists)
        {
            return BadRequest("Character name is already taken.");
        }

        // Set base stats based on class archetype
        var (baseHp, baseMana) = GetBaseStats(dto.CharacterClass);

        var character = new Character
        {
            Name = dto.Name,
            CharacterClass = dto.CharacterClass,
            Health = baseHp,
            Mana = baseMana,
            PlayerId = playerId.Value
        };

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        var response = new CharacterResponseDto(
            character.Id,
            character.Name,
            character.CharacterClass,
            character.Level,
            character.Experience,
            character.Health,
            character.Mana,
            character.CreatedAt
        );

        return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, response);
    }

    // POST: api/characters/{id}/gain-experience
    [HttpPost("{id:guid}/gain-experience")]
    public async Task<ActionResult<CharacterResponseDto>> AddExperience(Guid id, AddExperienceDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId.Value);

        if (character == null) return NotFound("Character not found.");

        character.Experience += dto.Amount;

        // Level-up curve: Level * 100 XP required per tier
        while (character.Experience >= GetXpRequiredForNextLevel(character.Level))
        {
            character.Experience -= GetXpRequiredForNextLevel(character.Level);
            character.Level++;

            // Stat growth per level
            character.Health += 20;
            character.Mana += 10;
        }

        await _context.SaveChangesAsync();

        return Ok(new CharacterResponseDto(
            character.Id,
            character.Name,
            character.CharacterClass,
            character.Level,
            character.Experience,
            character.Health,
            character.Mana,
            character.CreatedAt
        ));
    }

    // PUT: api/characters/{id}/stats
    [HttpPut("{id:guid}/stats")]
    public async Task<ActionResult<CharacterResponseDto>> UpdateStats(Guid id, UpdateStatsDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId.Value);

        if (character == null) return NotFound("Character not found.");

        character.Health = dto.Health;
        character.Mana = dto.Mana;

        await _context.SaveChangesAsync();

        return Ok(new CharacterResponseDto(
            character.Id,
            character.Name,
            character.CharacterClass,
            character.Level,
            character.Experience,
            character.Health,
            character.Mana,
            character.CreatedAt
        ));
    }

    // DELETE: api/characters/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacter(Guid id)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId.Value);

        if (character == null) return NotFound("Character not found.");

        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetCurrentPlayerId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var guid) ? guid : null;
    }

    private static int GetXpRequiredForNextLevel(int currentLevel) => currentLevel * 100;

    private static (int Health, int Mana) GetBaseStats(string characterClass) => characterClass.ToLower() switch
    {
        "mage" => (70, 120),
        "rogue" => (90, 60),
        "archer" => (85, 70),
        _ => (120, 40) // Default / Warrior
    };
}