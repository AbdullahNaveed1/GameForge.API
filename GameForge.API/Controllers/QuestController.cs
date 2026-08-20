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
public class QuestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/quests
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestResponseDto>>> GetQuests()
    {
        var quests = await _context.Quests
            .Include(q => q.GuaranteedItemReward)
            .Select(q => new QuestResponseDto(
                q.Id,
                q.Title,
                q.Description,
                q.RequiredLevel,
                q.ExperienceReward,
                q.GuaranteedItemReward != null ? q.GuaranteedItemReward.Name : null
            ))
            .ToListAsync();

        return Ok(quests);
    }

    // POST: api/quests/{questId}/complete/{characterId}
    [HttpPost("{questId:guid}/complete/{characterId:guid}")]
    public async Task<ActionResult<CompleteQuestResponseDto>> CompleteQuest(Guid questId, Guid characterId)
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(playerIdClaim, out var playerId))
        {
            return Unauthorized();
        }

        var character = await _context.Characters
            .Include(c => c.Inventory)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerId);

        if (character == null)
        {
            return NotFound("Character not found or unauthorized.");
        }

        var quest = await _context.Quests
            .Include(q => q.GuaranteedItemReward)
            .FirstOrDefaultAsync(q => q.Id == questId);

        if (quest == null)
        {
            return NotFound("Quest not found.");
        }

        if (character.Level < quest.RequiredLevel)
        {
            return BadRequest($"Character level is too low. Required Level: {quest.RequiredLevel}");
        }

        // 1. Award Experience & Calculate Level-Up
        character.Experience += quest.ExperienceReward;
        while (character.Experience >= (character.Level * 100))
        {
            character.Experience -= (character.Level * 100);
            character.Level++;
            character.Health += 20;
            character.Mana += 10;
        }

        // 2. Award Guaranteed Loot Reward
        string? grantedItemName = null;
        int grantedQuantity = 0;

        if (quest.GuaranteedItemReward != null)
        {
            grantedItemName = quest.GuaranteedItemReward.Name;
            grantedQuantity = 1;

            var existingStack = character.Inventory
                .FirstOrDefault(ii => ii.ItemId == quest.GuaranteedItemReward.Id && !ii.IsEquipped);

            if (existingStack != null && quest.GuaranteedItemReward.MaxStack > 1)
            {
                existingStack.Quantity = Math.Min(existingStack.Quantity + 1, quest.GuaranteedItemReward.MaxStack);
            }
            else
            {
                var newLootItem = new InventoryItem
                {
                    CharacterId = character.Id,
                    ItemId = quest.GuaranteedItemReward.Id,
                    Quantity = 1,
                    IsEquipped = false
                };
                _context.InventoryItems.Add(newLootItem);
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new CompleteQuestResponseDto(
            $"Quest '{quest.Title}' completed successfully!",
            quest.ExperienceReward,
            character.Level,
            character.Experience,
            grantedItemName,
            grantedQuantity
        ));
    }
}