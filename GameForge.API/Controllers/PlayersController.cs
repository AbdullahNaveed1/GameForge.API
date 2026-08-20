using GameForge.API.Data;
using GameForge.API.DTOs;
using GameForge.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameForge.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlayersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Players
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerResponseDto>>> GetPlayers()
    {
        return await _context.Players
            .Select(p => new PlayerResponseDto
            {
                Id = p.Id,
                Username = p.Username,
                Email = p.Email,
                Level = p.Level,
                Experience = p.Experience,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    // GET: api/Players/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerResponseDto>> GetPlayer(Guid id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        return new PlayerResponseDto
        {
            Id = player.Id,
            Username = player.Username,
            Email = player.Email,
            Level = player.Level,
            Experience = player.Experience,
            CreatedAt = player.CreatedAt
        };
    }

    // POST: api/Players
    [HttpPost]
    public async Task<ActionResult<PlayerResponseDto>> CreatePlayer([FromBody] CreatePlayerDto dto)
    {
        var player = new Player
        {
            Username = dto.Username,
            Email = dto.Email,
            Level = 1,
            Experience = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var response = new PlayerResponseDto
        {
            Id = player.Id,
            Username = player.Username,
            Email = player.Email,
            Level = player.Level,
            Experience = player.Experience,
            CreatedAt = player.CreatedAt
        };

        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, response);
    }

    // PUT: api/Players/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, [FromBody] UpdatePlayerDto dto)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        player.Level = dto.Level;
        player.Experience = dto.Experience;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Players/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(Guid id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}