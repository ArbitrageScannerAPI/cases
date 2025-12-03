using System.Net.Http.Json;
using System.Text.Json;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class PoolsService : IPoolsService
    {
        private readonly HttpClient _httpClient;
        
        // TODO: Replace with actual API URLs from configuration
        private const string SolanaPoolsApiUrl = "https://api.example.com/solana/pools";
        private const string EthereumPoolsApiUrl = "https://api.example.com/ethereum/pools";

        public PoolsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PoolInfo>> GetPoolsByTokenAsync(Network network, string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress))
            {
                return new List<PoolInfo>();
            }

            try
            {
                var baseUrl = network switch
                {
                    Network.Solana => SolanaPoolsApiUrl,
                    Network.Ethereum => EthereumPoolsApiUrl,
                    _ => throw new ArgumentException($"Unsupported network: {network}")
                };

                var url = $"{baseUrl}?tokenAddress={Uri.EscapeDataString(tokenAddress)}";
                Console.WriteLine($"Fetching pools: {url}");

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch pools: {response.StatusCode}");
                    return new List<PoolInfo>();
                }

                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Pools response: {body}");

                // TODO: Adjust deserialization based on actual API response structure
                // Option 1: Direct array response
                // var pools = JsonSerializer.Deserialize<List<PoolInfo>>(body);
                
                // Option 2: Wrapped response
                var apiResponse = JsonSerializer.Deserialize<PoolsApiResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return apiResponse?.Pools ?? new List<PoolInfo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching pools: {ex.Message}");
                return new List<PoolInfo>();
            }
        }
    }
}
