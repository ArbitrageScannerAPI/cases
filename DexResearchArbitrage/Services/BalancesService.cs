using System.Text.Json;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public class BalancesService : IBalancesService
    {
        private readonly HttpClient _httpClient;

        // Vercel proxy for address tokens balance (Solana)
        private const string SolanaBalancesProxyUrl = "https://vercel-apip-roxima.vercel.app/api/balances";

        // Vercel proxy for address tokens balance (Ethereum)
        private const string EthereumBalancesProxyUrl = "https://vercel-apip-roxima.vercel.app/api/eth_balances";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BalancesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AddressTokensBalanceResponse?> GetAddressTokensBalanceAsync(Network network, string poolAddress, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(poolAddress))
                return null;

            try
            {
                // Выбираем URL в зависимости от сети
                string baseUrl = network switch
                {
                    Network.Solana => SolanaBalancesProxyUrl,
                    Network.Ethereum => EthereumBalancesProxyUrl,
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(baseUrl)) return null;

                var url = $"{baseUrl}?address={Uri.EscapeDataString(poolAddress)}&limit={limit}";
                Console.WriteLine($"[{network} Balances] Calling proxy: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[{network} Balances] Status: {response.StatusCode}");
                // Console.WriteLine($"[{network} Balances] Body: {body}"); // Раскомментировать для отладки

                if (!response.IsSuccessStatusCode)
                    return null;

                // Используем те же модели, так как поле amount_usd совпадает в обоих JSON
                var result = JsonSerializer.Deserialize<AddressTokensBalanceResponse>(body, JsonOptions);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{network} Balances] ERROR: {ex.Message}");
                return null;
            }
        }
    }
}
