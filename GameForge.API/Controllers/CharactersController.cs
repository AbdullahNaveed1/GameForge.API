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
[Authorize] // Requires a valid JWT token for all endpoints
public class CharactersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CharactersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/characters
    // Returns all characters owned by the currently authenticated player
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

    // POST: api/characters
    // Creates a new character bound to the currently authenticated player
    [HttpPost]
    public async Task<ActionResult<CharacterResponseDto>> CreateCharacter(CreateCharacterDto dto)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        // Check if character name is already taken
        var nameExists = await _context.Characters.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());
        if (nameExists)
        {
            return BadRequest("Character name is already taken.");
        }

        var character = new Character
        {
            Name = dto.Name,
            CharacterClass = dto.CharacterClass,
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

        return CreatedAtAction(nameof(GetMyCharacters), new { id = character.Id }, response);
    }

    // DELETE: api/characters/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacter(Guid id)
    {
        var playerId = GetCurrentPlayerId();
        if (playerId == null) return Unauthorized("Invalid token claims.");

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.PlayerId == playerId.Value);

        if (character == null)
        {
            return NotFound("Character not found or does not belong to you.");
        }

        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Helper method to safely extract the Guid from the JWT ClaimTypes.NameIdentifier
    private Guid? GetCurrentPlayerId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var guid) ? guid : null;
    }
}