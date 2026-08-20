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
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/items
    // Optional filters: ?type=0 (Weapon) or ?rarity=4 (Legendary)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetAllItems(
        [FromQuery] ItemType? type,
        [FromQuery] ItemRarity? rarity)
    {
        var query = _context.Items.AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(i => i.Type == type.Value);
        }

        if (rarity.HasValue)
        {
            query = query.Where(i => i.Rarity == rarity.Value);
        }

        var items = await query
            .Select(i => new ItemResponseDto(
                i.Id,
                i.Name,
                i.Description,
                i.Type.ToString(),
                i.Rarity.ToString(),
                i.Value,
                i.MaxStack,
                i.AttackBonus,
                i.DefenseBonus,
                i.HealthRestore,
                i.ManaRestore
            ))
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/items/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemResponseDto>> GetItemById(Guid id)
    {
        var item = await _context.Items.FindAsync(id);

        if (item == null)
        {
            return NotFound("Item not found in catalog.");
        }

        return Ok(new ItemResponseDto(
            item.Id,
            item.Name,
            item.Description,
            item.Type.ToString(),
            item.Rarity.ToString(),
            item.Value,
            item.MaxStack,
            item.AttackBonus,
            item.DefenseBonus,
            item.HealthRestore,
            item.ManaRestore
        ));
    }
}