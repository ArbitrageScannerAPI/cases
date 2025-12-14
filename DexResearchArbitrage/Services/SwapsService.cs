using System.Text.Json;
using System.Text.Json.Serialization.Metadata; // Required for TypeInfoResolver
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class SwapsService : ISwapsService
    {
        private readonly HttpClient _httpClient;

        // Solana endpoint (existing: swops)
        private const string SolanaSwapsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/swops";

        // Ethereum endpoint (new: eth_swops)
        private const string EthereumSwapsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/eth_swops";

        // OPTION 1: Standard settings for Solana (uses [JsonPropertyName] from class)
        private static readonly JsonSerializerOptions SolanaOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // OPTION 2: Modified settings for Ethereum (maps 'token0/token1' to 'From/To')
        private static readonly JsonSerializerOptions EthOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ConfigureEthMapping }
            }
        };

        public SwapsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Modifier that renames JSON properties on the fly for Ethereum.
        /// It maps "token0_..." fields to "From..." properties and "token1_..." to "To..." properties.
        /// </summary>
        private static void ConfigureEthMapping(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Type != typeof(PoolSwapItem)) return;

            foreach (var p in typeInfo.Properties)
            {
                var memberName = (p.AttributeProvider as System.Reflection.MemberInfo)?.Name;

                if (memberName == nameof(PoolSwapItem.FromTokenAddress))
                    p.Name = "token0_address";
                else if (memberName == nameof(PoolSwapItem.FromTokenAmount))
                    p.Name = "token0_amount";
                else if (memberName == nameof(PoolSwapItem.ToTokenAddress))
                    p.Name = "token1_address";
                else if (memberName == nameof(PoolSwapItem.ToTokenAmount))
                    p.Name = "token1_amount";
            }
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

                PoolSwapsResponse? result = null;

                // Measure deserialization time
                var sw = System.Diagnostics.Stopwatch.StartNew();

                // Select correct JSON options based on network
                JsonSerializerOptions currentOptions = (network == Network.Ethereum) ? EthOptions : SolanaOptions;

                // Deserialize into the SAME class structure, but with different mapping rules
                result = JsonSerializer.Deserialize<PoolSwapsResponse>(body, currentOptions);

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
