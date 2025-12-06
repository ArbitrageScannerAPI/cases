using System.Text.Json;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class TokenPriceService : ITokenPriceService
    {
        private readonly HttpClient _httpClient;
        private const string LatestPriceProxyUrl = "https://vercel-apip-roxima.vercel.app/api/latestprice";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TokenPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LatestPriceResponse?> GetLatestPriceUsdAsync(string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress))
                return null;

            try
            {
                var url = $"{LatestPriceProxyUrl}?token_address={Uri.EscapeDataString(tokenAddress)}";
                Console.WriteLine($"[Price] Calling latest price proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Price] Status: {response.StatusCode}");
                Console.WriteLine($"[Price] Body: {body}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var price = JsonSerializer.Deserialize<LatestPriceResponse>(body, JsonOptions);
                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Price] ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
