using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DemoBlazor
{
    public class SolanaAddressValidator
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://b2b.api.arbitragescanner.io/api/onchain/v1/solana/address/general_info";
        private const string ApiKey = "JYnC6aZete6f6edaksMh5x";

        public SolanaAddressValidator(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ValidateAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            try
            {
                var url = $"{ApiUrl}?address={address}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("X-API-Key", ApiKey);

                var response = await _httpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response Status: {response.StatusCode}");
                Console.WriteLine($"API Response Body: {body}");

                // Проверяем код ответа 400 или наличие "error" в теле ответа
                if ((int)response.StatusCode == 400 || body.Contains("\"error\""))
                {
                    Console.WriteLine("Validation FAILED - Invalid address");
                    return false;
                }

                Console.WriteLine("Validation SUCCESS - Valid address");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Validation ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
