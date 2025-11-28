using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoBlazor
{
    public class SolanaAddressValidator
    {
        private readonly HttpClient _httpClient;
        
        // Ваш Vercel API URL
        private const string VercelApiUrl = "https://vercel-apip-roxima-5ta3rzjvs-igor-devs-projects.vercel.app/api/validate";

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
                var url = $"{VercelApiUrl}?address={Uri.EscapeDataString(address)}";
                
                Console.WriteLine($"Calling Vercel API: {url}");

                var response = await _httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Body: {body}");

                if ((int)response.StatusCode == 400 || body.Contains("\"error\""))
                {
                    Console.WriteLine("Validation FAILED");
                    return false;
                }

                Console.WriteLine("Validation SUCCESS");
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
