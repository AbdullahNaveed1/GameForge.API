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
    private const int MaxActiveCharacters = 4;

    public CharactersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/characters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterResponseDto>>> GetMyCharacters()
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized();

        var characters = await _context.Characters
            .Where(c => c.PlayerId == playerId)
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
        if (playerId == null) return Unauthorized();

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId);

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

    // GET: api/characters/{id}/stats
    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<CharacterCombatStatsDto>> GetCharacterCombatStats(Guid id)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized();

        var character = await _context.Characters
            .Include(c => c.Inventory)
                .ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId);

        if (character == null) return NotFound("Character not found.");

        // Dynamic base stats scaling with level and class
        int baseAttack = character.CharacterClass.ToLower() switch
        {
            "warrior" => 20 + (character.Level * 4),
            "mage" => 8 + (character.Level * 2),
            "rogue" => 15 + (character.Level * 3),
            _ => 10 + (character.Level * 2)
        };

        int baseDefense = character.CharacterClass.ToLower() switch
        {
            "warrior" => 15 + (character.Level * 3),
            "mage" => 5 + (character.Level * 1),
            "rogue" => 10 + (character.Level * 2),
            _ => 5 + (character.Level * 1)
        };

        // Aggregating gear bonuses
        var equippedItems = character.Inventory.Where(ii => ii.IsEquipped).ToList();

        int gearAttackBonus = equippedItems.Sum(ei => ei.Item.AttackBonus);
        int gearDefenseBonus = equippedItems.Sum(ei => ei.Item.DefenseBonus);

        var equippedGearDto = equippedItems.Select(ii => new InventoryItemResponseDto(
            ii.Id,
            ii.ItemId,
            ii.Item.Name,
            ii.Item.Description,
            ii.Item.Type.ToString(),
            ii.Item.Rarity.ToString(),
            ii.Quantity,
            ii.IsEquipped,
            ii.Item.AttackBonus,
            ii.Item.DefenseBonus,
            ii.Item.HealthRestore,
            ii.Item.ManaRestore
        )).ToList();

        var statsDto = new CharacterCombatStatsDto(
            character.Id,
            character.Name,
            character.CharacterClass,
            character.Level,
            character.Experience,
            character.Health,
            character.Mana,
            baseAttack,
            baseDefense,
            gearAttackBonus,
            gearDefenseBonus,
            baseAttack + gearAttackBonus,
            baseDefense + gearDefenseBonus,
            equippedGearDto
        );

        return Ok(statsDto);
    }

    // POST: api/characters
    [HttpPost]
    public async Task<ActionResult<CharacterResponseDto>> CreateCharacter(CreateCharacterDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized();

        var activeCount = await _context.Characters.CountAsync(c => c.PlayerId == playerId);
        if (activeCount >= MaxActiveCharacters)
        {
            return BadRequest($"Maximum character limit ({MaxActiveCharacters}) reached.");
        }

        var validClasses = new[] { "Warrior", "Mage", "Rogue" };
        if (!validClasses.Contains(dto.CharacterClass, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid class. Choose from Warrior, Mage, or Rogue.");
        }

        var character = new Character
        {
            Name = dto.Name,
            CharacterClass = dto.CharacterClass,
            PlayerId = playerId.Value
        };

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, new CharacterResponseDto(
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

    // PUT: api/characters/{id}/experience
    [HttpPut("{id:guid}/experience")]
    public async Task<ActionResult<CharacterResponseDto>> AddExperience(Guid id, AddExperienceDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized();

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId);

        if (character == null) return NotFound("Character not found.");

        character.Experience += dto.Amount;

        // Level-up curve calculation
        while (character.Experience >= (character.Level * 100))
        {
            character.Experience -= (character.Level * 100);
            character.Level++;
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

    // DELETE: api/characters/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacter(Guid id)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized();

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId);

        if (character == null) return NotFound("Character not found.");

        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetCurrentPlayerId()
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(playerIdClaim, out var playerId) ? playerId : null;
    }
}