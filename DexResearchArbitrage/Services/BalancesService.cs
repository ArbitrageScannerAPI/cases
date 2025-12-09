using System.Text.Json;
using System.Text.Json.Serialization;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class BalancesService : IBalancesService
    {
        private readonly HttpClient _httpClient;

        // Vercel proxy for address tokens balance
        private const string BalancesProxyUrl = "https://vercel-apip-roxima.vercel.app/api/balances";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BalancesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AddressTokensBalanceResponse?> GetAddressTokensBalanceAsync(string poolAddress, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(poolAddress))
                return null;

            try
            {
                var url = $"{BalancesProxyUrl}?address={Uri.EscapeDataString(poolAddress)}&limit={limit}";
                Console.WriteLine($"[Balances] Calling balances proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Balances] Status: {response.StatusCode}");
                Console.WriteLine($"[Balances] Body: {body}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = JsonSerializer.Deserialize<AddressTokensBalanceResponse>(body, JsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Balances] ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
