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
public class CombatController : ControllerBase
{
    private readonly AppDbContext _context;

    public CombatController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/combat/monsters
    [HttpGet("monsters")]
    public async Task<ActionResult<IEnumerable<MonsterResponseDto>>> GetMonsters()
    {
        var monsters = await _context.Monsters
            .Include(m => m.LootItem)
            .Select(m => new MonsterResponseDto(
                m.Id,
                m.Name,
                m.Level,
                m.MaxHealth,
                m.AttackPower,
                m.Defense,
                m.ExperienceReward,
                m.LootItem != null ? m.LootItem.Name : null
            ))
            .ToListAsync();

        return Ok(monsters);
    }

    // POST: api/combat/engage/{characterId}/{monsterId}
    [HttpPost("engage/{characterId:guid}/{monsterId:guid}")]
    public async Task<ActionResult<BattleResultDto>> EngageMonster(Guid characterId, Guid monsterId)
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(playerIdClaim, out var playerId))
        {
            return Unauthorized();
        }

        var character = await _context.Characters
            .Include(c => c.Inventory)
            .ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.PlayerId == playerId);

        if (character == null)
        {
            return NotFound("Character not found or unauthorized.");
        }

        var monster = await _context.Monsters
            .Include(m => m.LootItem)
            .FirstOrDefaultAsync(m => m.Id == monsterId);

        if (monster == null)
        {
            return NotFound("Monster not found.");
        }

        if (character.Health <= 0)
        {
            return BadRequest("Character has fallen and must rest or heal before battling.");
        }

        // Calculate Character equipment attack and defense bonuses
        int weaponAttackBonus = character.Inventory
            .Where(i => i.IsEquipped && i.Item != null && i.Item.Type == ItemType.Weapon)
            .Sum(i => i.Item!.AttackBonus);

        int armorDefenseBonus = character.Inventory
            .Where(i => i.IsEquipped && i.Item != null && i.Item.Type == ItemType.Armor)
            .Sum(i => i.Item!.DefenseBonus);

        int charTotalAttack = 10 + (character.Level * 2) + weaponAttackBonus;
        int charTotalDefense = 2 + (character.Level) + armorDefenseBonus;

        int monsterCurrentHealth = monster.MaxHealth;
        var logs = new List<BattleTurnLogDto>();
        int round = 1;

        // Turn-based battle loop
        while (character.Health > 0 && monsterCurrentHealth > 0 && round <= 20)
        {
            // 1. Character attacks Monster
            int playerDamage = Math.Max(1, charTotalAttack - monster.Defense);
            monsterCurrentHealth = Math.Max(0, monsterCurrentHealth - playerDamage);

            logs.Add(new BattleTurnLogDto(
                round,
                character.Name,
                monster.Name,
                playerDamage,
                monsterCurrentHealth
            ));

            if (monsterCurrentHealth <= 0)
            {
                break;
            }

            // 2. Monster retaliates against Character
            int monsterDamage = Math.Max(1, monster.AttackPower - charTotalDefense);
            character.Health = Math.Max(0, character.Health - monsterDamage);

            logs.Add(new BattleTurnLogDto(
                round,
                monster.Name,
                character.Name,
                monsterDamage,
                character.Health
            ));

            round++;
        }

        bool victory = monsterCurrentHealth <= 0;
        int awardedExp = 0;
        string? droppedItemName = null;

        if (victory)
        {
            awardedExp = monster.ExperienceReward;
            character.Experience += awardedExp;

            // Check level-up progression
            while (character.Experience >= (character.Level * 100))
            {
                character.Experience -= (character.Level * 100);
                character.Level++;
                character.Health += 20;
                character.Mana += 10;
            }

            // Roll for loot drop (50% chance if loot item assigned)
            var rng = new Random();
            if (monster.LootItem != null && rng.Next(1, 101) <= 50)
            {
                droppedItemName = monster.LootItem.Name;

                var existingStack = character.Inventory
                    .FirstOrDefault(ii => ii.ItemId == monster.LootItem.Id && !ii.IsEquipped);

                if (existingStack != null && monster.LootItem.MaxStack > 1)
                {
                    existingStack.Quantity = Math.Min(existingStack.Quantity + 1, monster.LootItem.MaxStack);
                }
                else
                {
                    _context.InventoryItems.Add(new InventoryItem
                    {
                        CharacterId = character.Id,
                        ItemId = monster.LootItem.Id,
                        Quantity = 1,
                        IsEquipped = false
                    });
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new BattleResultDto(
            victory,
            victory ? $"Victory against {monster.Name}!" : $"Defeated by {monster.Name}.",
            character.Health,
            awardedExp,
            character.Level,
            droppedItemName,
            logs
        ));
    }
}