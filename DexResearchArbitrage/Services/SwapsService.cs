using System.Text.Json;              // for JsonSerializerOptions and JsonSerializer
using DexResearchArbitrage.Models;   // for PoolSwapsResponse

namespace DexResearchArbitrage.Services
{
    public class SwapsService : ISwapsService
    {
        private readonly HttpClient _httpClient;
        
        // Solana endpoint (existing: swops)
        private const string SolanaSwapsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/swops";
        
        // Ethereum endpoint (new: eth_swops)
        private const string EthereumSwapsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/eth_swops";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SwapsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PoolSwapsResponse?> GetPoolSwapsAsync(Network network, string poolAddress, int limit = 3000)
        {
            if (string.IsNullOrWhiteSpace(poolAddress))
                return null;

            try
            {
                // Select URL based on network
                string baseUrl = network switch
                {
                    Network.Solana => SolanaSwapsProxyUrl,
                    Network.Ethereum => EthereumSwapsProxyUrl,
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(baseUrl)) return null;

                var url = $"{baseUrl}?pool_address={Uri.EscapeDataString(poolAddress)}&limit={limit}";
                Console.WriteLine($"[{network} Swaps] Calling swaps proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[{network} Swaps] Status: {response.StatusCode}");
                // Console.WriteLine($"[{network} Swaps] Body: {body}"); // Uncomment for debugging

                if (!response.IsSuccessStatusCode)
                    return null;

                // Measure deserialization time
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = JsonSerializer.Deserialize<PoolSwapsResponse>(body, JsonOptions);
                sw.Stop();

                if (result != null)
                {
                    Console.WriteLine($"[{network} Swaps] Deserialization time: {sw.ElapsedMilliseconds} ms, items: {result.Data.Count}");
                }
                else
                {
                    Console.WriteLine($"[{network} Swaps] Deserialization returned null");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{network} Swaps] ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
