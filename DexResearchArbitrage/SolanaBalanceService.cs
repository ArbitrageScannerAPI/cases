using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DemoBlazor
{
    public class SolanaBalanceService
    {
        private readonly HttpClient _httpClient;
        private const string VercelApiUrl = "https://vercel-apip-roxima.vercel.app/api/balances";
        
        // Адреса токенов
        private const string SOL_ADDRESS = "So11111111111111111111111111111111111111111";
        private const string WSOL_ADDRESS = "So11111111111111111111111111111111111111112";
        private const string USDC_ADDRESS = "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v";

        public SolanaBalanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WalletBalances> GetBalancesAsync(string address)
{
    if (string.IsNullOrWhiteSpace(address))
    {
        return new WalletBalances();
    }

    try
    {
        var url = $"{VercelApiUrl}?address={Uri.EscapeDataString(address)}&limit=50";
        
        Console.WriteLine($"Fetching balances from: {url}");

        var response = await _httpClient.GetFromJsonAsync<TokenBalanceResponse>(url);

        if (response?.Data == null)
        {
            Console.WriteLine("No balance data received");
            return new WalletBalances();
        }

        var balances = new WalletBalances();

        // SOL
        var solToken = response.Data.FirstOrDefault(t => t.TokenAddress == SOL_ADDRESS);
        if (solToken != null)
        {
            balances.Sol = solToken.Amount; // 
            Console.WriteLine($"SOL: {balances.Sol}, USD: {solToken.AmountUsd}");
            balances.TotalUsd += solToken.AmountUsd ?? 0;
        }

        // wSOL
        var wsolToken = response.Data.FirstOrDefault(t => t.TokenAddress == WSOL_ADDRESS);
        if (wsolToken != null)
        {
            balances.WSol = wsolToken.Amount;
            Console.WriteLine($"wSOL: {balances.WSol}, USD: {wsolToken.AmountUsd}");
            balances.TotalUsd += wsolToken.AmountUsd ?? 0;
        }

        // USDC
        var usdcToken = response.Data.FirstOrDefault(t => t.TokenAddress == USDC_ADDRESS);
        if (usdcToken != null)
        {
            balances.Usdc = usdcToken.Amount;
            Console.WriteLine($"USDC: {balances.Usdc}, USD: {usdcToken.AmountUsd}");
            balances.TotalUsd += usdcToken.AmountUsd ?? 0;
        }

        Console.WriteLine($"Total USD: {balances.TotalUsd}");

        return balances;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching balances: {ex.Message}");
        return new WalletBalances();
    }
}

    }
}
