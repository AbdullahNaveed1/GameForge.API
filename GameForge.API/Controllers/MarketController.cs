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
public class MarketController : ControllerBase
{
    private readonly AppDbContext _context;

    public MarketController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/market/listings
    [HttpGet("listings")]
    public async Task<ActionResult<IEnumerable<AuctionListingResponseDto>>> GetActiveListings()
    {
        var listings = await _context.AuctionListings
            .Include(a => a.SellerCharacter)
            .Include(a => a.Item)
            .Where(a => !a.IsSold && !a.IsCancelled)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(a => new AuctionListingResponseDto(
                a.Id,
                a.SellerCharacterId,
                a.SellerCharacter != null ? a.SellerCharacter.Name : "Unknown Merchant",
                a.ItemId,
                a.Item != null ? a.Item.Name : "Unknown Relic",
                a.Quantity,
                a.PriceInGold,
                a.CreatedAtUtc
            ))
            .ToListAsync();

        return Ok(listings);
    }

    // POST: api/market/list
    [HttpPost("list")]
    public async Task<ActionResult> CreateListing([FromBody] CreateAuctionRequestDto request)
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(playerIdClaim, out var playerId))
        {
            return Unauthorized();
        }

        if (request.Quantity <= 0 || request.PriceInGold <= 0)
        {
            return BadRequest("Quantity and Price must be greater than zero.");
        }

        var character = await _context.Characters
            .Include(c => c.Inventory)
            .FirstOrDefaultAsync(c => c.Id == request.SellerCharacterId && c.PlayerId == playerId);

        if (character == null)
        {
            return NotFound("Character not found or unauthorized.");
        }

        var inventoryItem = character.Inventory
            .FirstOrDefault(ii => ii.ItemId == request.ItemId && !ii.IsEquipped);

        if (inventoryItem == null || inventoryItem.Quantity < request.Quantity)
        {
            return BadRequest("Character does not have enough unequipped items in inventory to list.");
        }

        // Deduct item from inventory into escrow
        inventoryItem.Quantity -= request.Quantity;
        if (inventoryItem.Quantity <= 0)
        {
            _context.InventoryItems.Remove(inventoryItem);
        }

        var auction = new AuctionListing
        {
            SellerCharacterId = character.Id,
            ItemId = request.ItemId,
            Quantity = request.Quantity,
            PriceInGold = request.PriceInGold,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.AuctionListings.Add(auction);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Item listed on auction house successfully.", AuctionId = auction.Id });
    }

    // POST: api/market/buy/{listingId}/{buyerCharacterId}
    [HttpPost("buy/{listingId:guid}/{buyerCharacterId:guid}")]
    public async Task<ActionResult<MarketTransactionResultDto>> BuyItem(Guid listingId, Guid buyerCharacterId)
    {
        var playerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(playerIdClaim, out var playerId))
        {
            return Unauthorized();
        }

        var buyer = await _context.Characters
            .Include(c => c.Inventory)
            .FirstOrDefaultAsync(c => c.Id == buyerCharacterId && c.PlayerId == playerId);

        if (buyer == null)
        {
            return NotFound("Buyer character not found or unauthorized.");
        }

        var listing = await _context.AuctionListings
            .Include(a => a.Item)
            .Include(a => a.SellerCharacter)
            .FirstOrDefaultAsync(a => a.Id == listingId);

        if (listing == null || listing.IsSold || listing.IsCancelled)
        {
            return NotFound("Auction listing is no longer available.");
        }

        if (listing.SellerCharacterId == buyer.Id)
        {
            return BadRequest("You cannot buy your own auction listing.");
        }

        if (buyer.Gold < listing.PriceInGold)
        {
            return BadRequest($"Insufficient gold balance. Required: {listing.PriceInGold} Gold, Current: {buyer.Gold} Gold.");
        }

        // 1. Transfer Gold
        buyer.Gold -= listing.PriceInGold;
        if (listing.SellerCharacter != null)
        {
            listing.SellerCharacter.Gold += listing.PriceInGold;
        }

        // 2. Deliver Item to Buyer
        var existingStack = buyer.Inventory
            .FirstOrDefault(ii => ii.ItemId == listing.ItemId && !ii.IsEquipped);

        if (existingStack != null && listing.Item != null && listing.Item.MaxStack > 1)
        {
            existingStack.Quantity += listing.Quantity;
        }
        else
        {
            _context.InventoryItems.Add(new InventoryItem
            {
                CharacterId = buyer.Id,
                ItemId = listing.ItemId,
                Quantity = listing.Quantity,
                IsEquipped = false
            });
        }

        // 3. Mark Listing as Sold
        listing.IsSold = true;

        await _context.SaveChangesAsync();

        return Ok(new MarketTransactionResultDto(
            true,
            $"Purchased {listing.Quantity}x {listing.Item?.Name} for {listing.PriceInGold} Gold!",
            buyer.Gold,
            listing.PriceInGold
        ));
    }
}