using System.Text.Json.Serialization;

namespace DexResearchArbitrage.Models
{
    // Matches ArbitrageScanner /token/liquidity_pools response
    public class LiquidityPoolsApiResponse
    {
        [JsonPropertyName("meta")]
        public LiquidityPoolsMeta Meta { get; set; } = new();

        [JsonPropertyName("data")]
        public List<LiquidityPoolItem> Data { get; set; } = new();
    }

    public class LiquidityPoolsMeta
    {
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool HasNextPage { get; set; }
    }

    
    public class LatestPriceResponse
    {
        [JsonPropertyName("price_usd")]
        public decimal PriceUsd { get; set; }
    }

    public class LiquidityPoolItem
    {
        [JsonPropertyName("pool_address")]
        public string PoolAddress { get; set; } = string.Empty;

        [JsonPropertyName("dex")]
        public string Dex { get; set; } = string.Empty;

        [JsonPropertyName("token0")]
        public LiquidityPoolTokenInfo Token0 { get; set; } = new();

        [JsonPropertyName("token0_price_usd")]
        public decimal? Token0PriceUsd { get; set; }

        [JsonPropertyName("token1")]
        public LiquidityPoolTokenInfo Token1 { get; set; } = new();

        [JsonPropertyName("token1_price_usd")]
        public decimal? Token1PriceUsd { get; set; }

        // Was: public DateTime CreatedAtTimestamp { get; set; }
        // Some pools return null here, so it must be nullable.
        [JsonPropertyName("created_at_timestamp")]
        public DateTime? CreatedAtTimestamp { get; set; }
    }

 
    public class LiquidityPoolTokenInfo
    {
        [JsonPropertyName("token_address")]
        public string TokenAddress { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    // Existing view model used by UI
    public class PoolInfo
{
    public string Dex { get; set; } = string.Empty;
    public string PoolAddress { get; set; } = string.Empty;
    public string SecondTokenAddress { get; set; } = string.Empty;
    public string? SecondTokenSymbol { get; set; }

    public decimal TvlUsd { get; set; }
    public int CountSwaps { get; set; }
    public decimal PriceDiffPercent { get; set; }
    public bool LatestPriceFlag { get; set; }
    public DateTime? LastSwapTimestamp { get; set; }

    // Новый флаг — идёт ли пересчёт TVL по этому пулу
    public bool IsTvlLoading { get; set; }
    public bool IsSwapsLoading { get; set; }

    // Latest USD price of the first token for this pool (used for LatestPrice column)
    public decimal LatestPricePriceUsd { get; set; }

    // Cached price history: first/second price per swap for this pool
    public List<SwapPricePoint> PriceHistory { get; set; } = new();
    

}



    
    /// <summary>
    /// API response wrapper for pools endpoint
    /// </summary>
    public class PoolsApiResponse
    {
        [JsonPropertyName("pools")]
        public List<PoolInfo> Pools { get; set; } = new();
        
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        
        // TODO: Adjust based on actual API response structure
    }

    public class AddressTokensBalanceResponse
{
    [JsonPropertyName("meta")]
    public AddressTokensMeta Meta { get; set; } = new();

    [JsonPropertyName("data")]
    public List<AddressTokenBalanceItem> Data { get; set; } = new();
}

public class AddressTokensMeta
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("has_next_page")]
    public bool HasNextPage { get; set; }
}

public class AddressTokenBalanceItem
{
    [JsonPropertyName("token_address")]
    public string TokenAddress { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("amount_usd")]
    public decimal? AmountUsd { get; set; }
}

public class PoolSwapsResponse
{
    [JsonPropertyName("meta")]
    public PoolSwapsMeta Meta { get; set; } = new();

    [JsonPropertyName("data")]
    public List<PoolSwapItem> Data { get; set; } = new();
}

public class PoolSwapsMeta
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("has_next_page")]
    public bool HasNextPage { get; set; }
}

public class PoolSwapItem
{
    [JsonPropertyName("tx")]
    public string Tx { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("from_token_address")]
    public string FromTokenAddress { get; set; } = string.Empty;

    [JsonPropertyName("from_token_amount")]
    public decimal FromTokenAmount { get; set; }

    [JsonPropertyName("to_token_address")]
    public string ToTokenAddress { get; set; } = string.Empty;

    [JsonPropertyName("to_token_amount")]
    public decimal ToTokenAmount { get; set; }
}

// Price history item: price of "first token" in terms of "second token" (or USD)
public class SwapPricePoint
{
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }
}


}
