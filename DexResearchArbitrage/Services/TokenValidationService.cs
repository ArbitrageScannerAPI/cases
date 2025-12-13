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

    // DTO для Ethereum-ответа
    internal class EthereumTokenOkResponse
    {
        [JsonPropertyName("token_address")]
        public string TokenAddress { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("price_usd")]
        public decimal? PriceUsd { get; set; }

        [JsonPropertyName("total_supply")]
        public decimal? TotalSupply { get; set; }

        [JsonPropertyName("deployed_at_block_id")]
        public long? DeployedAtBlockId { get; set; }

        [JsonPropertyName("deployed_at_timestamp")]
        public DateTime? DeployedAtTimestamp { get; set; }
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

        // Ethereum validation endpoint via proxy (у тебя уже настроен).
        private const string EthereumValidationApiUrl = "https://vercel-apip-roxima.vercel.app/api/eth_validate";

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
        /// Validates Ethereum token using the configured proxy.
        /// JSON format example:
        /// {
        ///   "token_address": "...",
        ///   "name": "Tether USD",
        ///   "symbol": "USDT",
        ///   "decimals": 6,
        ///   "price_usd": 1.00009022,
        ///   "total_supply": 102696605277.3862,
        ///   "deployed_at_block_id": 4634748,
        ///   "deployed_at_timestamp": "2017-11-28T00:41:21"
        /// }
        /// </summary>
        private async Task<TokenValidationResult> ValidateEthereumTokenAsync(string tokenAddress)
        {
            try
            {
                // Простейшая проверка формата адреса перед запросом
                // (по твоему требованию можно было и не делать, но это дешевая защита от мусора).
                var trimmed = tokenAddress.Trim();
                if (!trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || trimmed.Length != 42)
                {
                    Console.WriteLine("[Ethereum] Local format validation FAILED");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Invalid Ethereum token address format"
                    };
                }

                // Вызов прокси-эндоинта, куда ты уже прописал адрес.
                // Если у тебя другой контракт (например, POST body) — просто поменяй URL/метод.
                var url = $"{EthereumValidationApiUrl}?address={Uri.EscapeDataString(trimmed)}";
                Console.WriteLine($"[Ethereum] Calling ETH validation API: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[Ethereum] Response Status: {response.StatusCode}");
                Console.WriteLine($"[Ethereum] Response Body: {body}");

                // Любой не-200 статус трактуем как ошибка, как ты и просил
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[Ethereum] Validation FAILED: non-success HTTP status");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Invalid Ethereum token address or API error"
                    };
                }

                // Парсим JSON в EthereumTokenOkResponse
                EthereumTokenOkResponse? ok;
                try
                {
                    ok = JsonSerializer.Deserialize<EthereumTokenOkResponse>(body, JsonOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Ethereum] Parse OK response error: {ex.Message}");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Failed to parse Ethereum token information from API"
                    };
                }

                // Проверка, что объект не null и есть token_address
                if (ok == null || string.IsNullOrWhiteSpace(ok.TokenAddress))
                {
                    Console.WriteLine("[Ethereum] OK response is missing token_address");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Ethereum token information is incomplete in API response"
                    };
                }

                // Сравнение адресов (case-insensitive)
                if (!string.Equals(ok.TokenAddress, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[Ethereum] Response token_address mismatch");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        TokenAddress = tokenAddress,
                        ErrorMessage = "Ethereum token address mismatch in API response"
                    };
                }

                Console.WriteLine("[Ethereum] Validation SUCCESS");

                // Ничего дополнительно не запоминаем, только базовые поля
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
                Console.WriteLine($"[Ethereum] Validation ERROR: {ex.Message}");
                return new TokenValidationResult
                {
                    IsValid = false,
                    TokenAddress = tokenAddress,
                    ErrorMessage = $"Ethereum validation error: {ex.Message}"
                };
            }
        }
    }
}
