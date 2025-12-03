using System.Text.Json.Serialization;

namespace DexResearchArbitrage.Models
{
    public class PoolInfo
    {
        [JsonPropertyName("dex")]
        public string Dex { get; set; } = string.Empty;
        
        [JsonPropertyName("poolAddress")]
        public string PoolAddress { get; set; } = string.Empty;
        
        [JsonPropertyName("secondTokenAddress")]
        public string SecondTokenAddress { get; set; } = string.Empty;
        
        [JsonPropertyName("secondTokenSymbol")]
        public string? SecondTokenSymbol { get; set; }
        
        [JsonPropertyName("tvlUsd")]
        public decimal TvlUsd { get; set; }
        
        [JsonPropertyName("countSwaps")]
        public int CountSwaps { get; set; }
        
        [JsonPropertyName("priceDiffPercent")]
        public decimal PriceDiffPercent { get; set; }
        
        [JsonPropertyName("arbitrationFlag")]
        public bool ArbitrationFlag { get; set; }
        
        [JsonPropertyName("lastSwapTimestamp")]
        public DateTime? LastSwapTimestamp { get; set; }
        
        // TODO: Add additional fields based on actual API response structure
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
