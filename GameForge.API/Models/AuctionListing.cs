namespace GameForge.API.Models;

public class AuctionListing
{
    public Guid Id { get; set; }

    public Guid SellerCharacterId { get; set; }
    public Character? SellerCharacter { get; set; }

    public Guid ItemId { get; set; }
    public Item? Item { get; set; }

    public int Quantity { get; set; } = 1;
    public int PriceInGold { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsSold { get; set; } = false;
    public bool IsCancelled { get; set; } = false;
}