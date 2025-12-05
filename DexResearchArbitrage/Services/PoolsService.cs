using System.Text.Json;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class PoolsService : IPoolsService
    {
        private readonly HttpClient _httpClient;

        // Vercel proxy URL for liquidity pools
        // It wraps ArbitrageScanner /solana/token/liquidity_pools with limit=200.
        private const string SolanaPoolsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/liquidity-pools";

        // TODO: Add Ethereum pools endpoint / proxy when available.
        private const string EthereumPoolsApiUrl = "https://api.example.com/ethereum/pools";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PoolsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PoolInfo>> GetPoolsByTokenAsync(Network network, string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress))
                return new List<PoolInfo>();

            return network switch
            {
                Network.Solana   => await GetSolanaPoolsAsync(tokenAddress),
                Network.Ethereum => await GetEthereumPoolsStubAsync(tokenAddress),
                _ => new List<PoolInfo>()
            };
        }

        private async Task<List<PoolInfo>> GetSolanaPoolsAsync(string tokenAddress)
        {
            try
            {
                // Vercel proxy expects "token_address" query parameter.
                var url = $"{SolanaPoolsProxyUrl}?token_address={Uri.EscapeDataString(tokenAddress)}";
                Console.WriteLine($"[Solana Pools] Calling Vercel liquidity proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Solana Pools] Response Status: {response.StatusCode}");
                Console.WriteLine($"[Solana Pools] Response Body: {body}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[Solana Pools] Non-success status, returning empty list");
                    return new List<PoolInfo>();
                }

                var apiResponse = JsonSerializer.Deserialize<LiquidityPoolsApiResponse>(body, JsonOptions);
                if (apiResponse == null || apiResponse.Data.Count == 0)
                    return new List<PoolInfo>();

                // has_next_page == false with limit=200 means we already have full set.
                // Map raw API pools to UI PoolInfo objects.
                var result = new List<PoolInfo>();

                foreach (var p in apiResponse.Data)
                {
                    // Determine which token is "second" relative to searched tokenAddress
                    // If token0 == searched token, second is token1, otherwise token0.
                    bool token0IsSearched =
                        string.Equals(p.Token0.TokenAddress, tokenAddress, StringComparison.OrdinalIgnoreCase);

                    var second = token0IsSearched ? p.Token1 : p.Token0;

                    result.Add(new PoolInfo
                    {
                        Dex = p.Dex,
                        PoolAddress = p.PoolAddress,
                        SecondTokenAddress = second.TokenAddress,
                        SecondTokenSymbol = second.Symbol,
                        // TVL, CountSwaps, PriceDiffPercent, ArbitrationFlag will be filled later
                        // when a richer pools / stats endpoint is integrated.
                        TvlUsd = 0,
                        CountSwaps = 0,
                        PriceDiffPercent = 0,
                        ArbitrationFlag = false,
                        LastSwapTimestamp = p.CreatedAtTimestamp
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Solana Pools] ERROR: {ex.Message}");
                return new List<PoolInfo>();
            }
        }

        private async Task<List<PoolInfo>> GetEthereumPoolsStubAsync(string tokenAddress)
        {
            // TODO: Implement Ethereum pools fetching via ArbitrageScanner or another provider.
            // For now we return an empty list to indicate no pools.
            await Task.CompletedTask;
            return new List<PoolInfo>();
        }
    }
}
