using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    // DTOs for the Vercel/ArbitrageScanner API response
    // NOTE: These classes are internal to the validation service and can be adjusted
    // if the external API changes its schema.

    internal class SolanaTokenOkResponse
    {
        [JsonPropertyName("token_address")]
        public string TokenAddress { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }
    }

    internal class SolanaTokenErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;
    }

    public class TokenValidationService : ITokenValidationService
    {
        private readonly HttpClient _httpClient;

        // Production Vercel proxy URL (must stay in sync with the original project).
        // The proxy calls ArbitrageScanner API and, depending on the "type" parameter,
        // can use:
        //   - /address/general_info  (type omitted or "address")
        //   - /token/general_info    (type = "token")  <-- used by this service.
        private const string VercelApiUrl = "https://vercel-apip-roxima.vercel.app/api/validate";

        // TODO: Replace with a real Ethereum validation endpoint when it becomes available.
        private const string EthereumValidationApiUrl = "https://api.example.com/ethereum/token/general_info";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TokenValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenValidationResult> ValidateTokenAsync(Network network, string tokenAddress)
        {
            if (string.IsNullOrWhiteSpace(tokenAddress))
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token address cannot be empty"
                };
            }

            return network switch
            {
                Network.Solana   => await ValidateSolanaTokenAsync(tokenAddress),
                Network.Ethereum => await ValidateEthereumTokenAsync(tokenAddress),
                _ => new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Unsupported network: {network}"
                }
            };
        }

        /// <summary>
        /// Validates Solana token using the existing Vercel proxy.
        /// The proxy wraps ArbitrageScanner API and returns either:
        /// - error JSON: { "error": "...", "detail": "..." }
        /// - success JSON: { "token_address": "...", "name": "...", "symbol": "...", "decimals": 6 }
        /// </summary>
        private async Task<TokenValidationResult> ValidateSolanaTokenAsync(string tokenAddress)
        {
            try
            {
                // IMPORTANT:
                // We add type=token so the Vercel proxy calls the "token/general_info" endpoint
                // instead of "address/general_info".
                var url = $"{VercelApiUrl}?address={Uri.EscapeDataString(tokenAddress)}&type=token";
                Console.WriteLine($"[Solana] Calling Vercel API: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Solana] Response Status: {response.StatusCode}");
                Console.WriteLine($"[Solana] Response Body: {body}");

                // HTTP 400 or explicit error field => invalid token
                if (!response.IsSuccessStatusCode || body.Contains("\"error\""))
                {
                    // Try to parse error details for better UX
                    SolanaTokenErrorResponse? error = null;
                    try
                    {
                        error = JsonSerializer.Deserialize<SolanaTokenErrorResponse>(body, JsonOptions);
                    }
                    catch
                    {
                        // ignore parse error, we still have raw body
                    }

                    var message = error?.Detail
                                  ?? error?.Error
                                  ?? "Invalid Solana token address";

                    Console.WriteLine($"[Solana] Validation FAILED: {message}");

                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = message
                    };
                }

                // Try to parse success payload
                SolanaTokenOkResponse? ok;
                try
                {
                    ok = JsonSerializer.Deserialize<SolanaTokenOkResponse>(body, JsonOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Solana] Parse OK response error: {ex.Message}");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Failed to parse token information from API"
                    };
                }

                if (ok == null || string.IsNullOrWhiteSpace(ok.TokenAddress))
                {
                    Console.WriteLine("[Solana] OK response is missing token_address");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Token information is incomplete in API response"
                    };
                }

                Console.WriteLine("[Solana] Validation SUCCESS");

                return new TokenValidationResult
                {
                    IsValid = true,
                    TokenAddress = ok.TokenAddress,
                    TokenSymbol = ok.Symbol,
                    TokenName = ok.Name
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Solana] Validation ERROR: {ex.Message}");
                return new TokenValidationResult
                {
                    IsValid = false,
                    TokenAddress = tokenAddress,
                    ErrorMessage = $"Validation error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Placeholder for Ethereum token validation.
        /// Currently not implemented; once Ethereum endpoint is available,
        /// mirror the Solana logic here using the corresponding API.
        /// </summary>
        private async Task<TokenValidationResult> ValidateEthereumTokenAsync(string tokenAddress)
        {
            // TODO: Implement Ethereum validation via ArbitrageScanner or another provider.
            // For now we simply mark it as not supported.
            await Task.CompletedTask;

            return new TokenValidationResult
            {
                IsValid = false,
                TokenAddress = tokenAddress,
                ErrorMessage = "Ethereum token validation is not implemented yet"
            };
        }
    }
}
