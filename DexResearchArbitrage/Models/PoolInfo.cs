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

        [JsonPropertyName("created_at_timestamp")]
        public DateTime CreatedAtTimestamp { get; set; }
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

        // Second token in pair relative to searched token
        public string SecondTokenAddress { get; set; } = string.Empty;
        public string? SecondTokenSymbol { get; set; }

        // For now TVL and swaps are not provided by this endpoint,
        // so we keep them as 0 until a richer API is integrated.
        public decimal TvlUsd { get; set; }
        public int CountSwaps { get; set; }

        public decimal PriceDiffPercent { get; set; }
        public bool ArbitrationFlag { get; set; }

        public DateTime? LastSwapTimestamp { get; set; }
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
}
