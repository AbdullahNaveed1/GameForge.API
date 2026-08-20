using System.Security.Claims;
using GameForge.API.Data;
using GameForge.API.DTOs;
using GameForge.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameForge.API.Controllers;

[ApiController]
[Route("api/characters/{characterId:guid}/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private const int MaxInventorySlots = 20;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/characters/{characterId}/inventory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemResponseDto>>> GetInventory(Guid characterId)
    {
        var character = await VerifyCharacterOwnership(characterId);
        if (character == null) return NotFound("Character not found or unauthorized.");

        var inventory = await _context.InventoryItems
            .Where(ii => ii.CharacterId == characterId)
            .Include(ii => ii.Item)
            .Select(ii => new InventoryItemResponseDto(
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
            ))
            .ToListAsync();

        return Ok(inventory);
    }

    // POST: api/characters/{characterId}/inventory
    [HttpPost]
    public async Task<ActionResult<InventoryItemResponseDto>> AddItemToInventory(
        Guid characterId,
        AddInventoryItemDto dto)
    {
        var character = await VerifyCharacterOwnership(characterId);
        if (character == null) return NotFound("Character not found or unauthorized.");

        var item = await _context.Items.FindAsync(dto.ItemId);
        if (item == null) return NotFound("Item does not exist in catalog.");

        // Check if stackable item already exists in inventory
        var existingInventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(ii => ii.CharacterId == characterId && ii.ItemId == dto.ItemId && !ii.IsEquipped);

        if (existingInventoryItem != null && item.MaxStack > 1)
        {
            existingInventoryItem.Quantity = Math.Min(existingInventoryItem.Quantity + dto.Quantity, item.MaxStack);
            await _context.SaveChangesAsync();

            return Ok(new InventoryItemResponseDto(
                existingInventoryItem.Id,
                item.Id,
                item.Name,
                item.Description,
                item.Type.ToString(),
                item.Rarity.ToString(),
                existingInventoryItem.Quantity,
                existingInventoryItem.IsEquipped,
                item.AttackBonus,
                item.DefenseBonus,
                item.HealthRestore,
                item.ManaRestore
            ));
        }

        // Enforce inventory slot capacity
        var currentSlotCount = await _context.InventoryItems.CountAsync(ii => ii.CharacterId == characterId);
        if (currentSlotCount >= MaxInventorySlots)
        {
            return BadRequest($"Inventory is full (Max {MaxInventorySlots} slots).");
        }

        var newInventoryItem = new InventoryItem
        {
            CharacterId = characterId,
            ItemId = dto.ItemId,
            Quantity = Math.Min(dto.Quantity, item.MaxStack),
            IsEquipped = false
        };

        _context.InventoryItems.Add(newInventoryItem);
        await _context.SaveChangesAsync();

        return Ok(new InventoryItemResponseDto(
            newInventoryItem.Id,
            item.Id,
            item.Name,
            item.Description,
            item.Type.ToString(),
            item.Rarity.ToString(),
            newInventoryItem.Quantity,
            newInventoryItem.IsEquipped,
            item.AttackBonus,
            item.DefenseBonus,
            item.HealthRestore,
            item.ManaRestore
        ));
    }

    // PUT: api/characters/{characterId}/inventory/{inventoryItemId}/toggle-equip
    [HttpPut("{inventoryItemId:guid}/toggle-equip")]
    public async Task<IActionResult> ToggleEquipItem(Guid characterId, Guid inventoryItemId)
    {
        var character = await VerifyCharacterOwnership(characterId);
        if (character == null) return NotFound("Character not found or unauthorized.");

        var inventoryItem = await _context.InventoryItems
            .Include(ii => ii.Item)
            .FirstOrDefaultAsync(ii => ii.Id == inventoryItemId && ii.CharacterId == characterId);

        if (inventoryItem == null) return NotFound("Item not found in inventory.");

        if (inventoryItem.Item.Type == ItemType.Consumable || inventoryItem.Item.Type == ItemType.Material)
        {
            return BadRequest("Cannot equip consumable or material items.");
        }

        // If equipping, unequip any other item of the same type currently equipped
        if (!inventoryItem.IsEquipped)
        {
            var equippedSameType = await _context.InventoryItems
                .Include(ii => ii.Item)
                .Where(ii => ii.CharacterId == characterId && ii.IsEquipped && ii.Item.Type == inventoryItem.Item.Type)
                .ToListAsync();

            foreach (var item in equippedSameType)
            {
                item.IsEquipped = false;
            }
        }

        inventoryItem.IsEquipped = !inventoryItem.IsEquipped;
        await _context.SaveChangesAsync();

        return Ok(new { Message = inventoryItem.IsEquipped ? "Item equipped." : "Item unequipped.", inventoryItem.IsEquipped });
    }

    // DELETE: api/characters/{characterId}/inventory/{inventoryItemId}
    [HttpDelete("{inventoryItemId:guid}")]
    public async Task<IActionResult> RemoveInventoryItem(Guid characterId, Guid inventoryItemId)
    {
        var character = await VerifyCharacterOwnership(characterId);
        if (character == null) return NotFound("Character not found or unauthorized.");

        var inventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(ii => ii.Id == inventoryItemId && ii.CharacterId == characterId);

        if (inventoryItem == null) return NotFound("Item not found in inventory.");

        _context.InventoryItems.Remove(inventoryItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Character?> VerifyCharacterOwnership(Guid characterId)
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(playerIdClaim, out var playerId)) return null;

        return await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerId);
    }
}