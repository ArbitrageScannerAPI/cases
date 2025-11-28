using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DemoBlazor
{
    public class TokenBalanceResponse
    {
        [JsonPropertyName("meta")]
        public Meta? Meta { get; set; }

        [JsonPropertyName("data")]
        public List<TokenBalance>? Data { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool HasNextPage { get; set; }
    }

    public class TokenBalance
    {
        [JsonPropertyName("token_address")]
        public string? TokenAddress { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("amount_usd")]
        public decimal? AmountUsd { get; set; }
    }

    public class WalletBalances
    {
        public decimal Sol { get; set; }
        public decimal WSol { get; set; }
        public decimal Usdc { get; set; }
        public decimal TotalUsd { get; set; }
    }
}
