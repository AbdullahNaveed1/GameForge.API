namespace GameForge.API.DTOs;

public record CreateAuctionRequestDto(
    Guid SellerCharacterId,
    Guid ItemId,
    int Quantity,
    int PriceInGold
);

public record AuctionListingResponseDto(
    Guid Id,
    Guid SellerCharacterId,
    string SellerName,
    Guid ItemId,
    string ItemName,
    int Quantity,
    int PriceInGold,
    DateTime CreatedAtUtc
);

public record MarketTransactionResultDto(
    bool Success,
    string Message,
    int RemainingBuyerGold,
    int SellerEarnedGold
);