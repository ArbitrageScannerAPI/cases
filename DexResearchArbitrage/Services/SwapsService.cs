using System.Text.Json;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class SwapsService : ISwapsService
    {
        private readonly HttpClient _httpClient;

        private const string SwapsProxyUrl = "https://vercel-apip-roxima.vercel.app/api/swops";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SwapsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PoolSwapsResponse?> GetPoolSwapsAsync(string poolAddress, int limit = 10000)
        {
            if (string.IsNullOrWhiteSpace(poolAddress))
                return null;

            try
            {
                var url = $"{SwapsProxyUrl}?pool_address={Uri.EscapeDataString(poolAddress)}&limit={limit}";
                Console.WriteLine($"[Swaps] Calling swaps proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Swaps] Status: {response.StatusCode}");
                Console.WriteLine($"[Swaps] Body: {body}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = JsonSerializer.Deserialize<PoolSwapsResponse>(body, JsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Swaps] ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
