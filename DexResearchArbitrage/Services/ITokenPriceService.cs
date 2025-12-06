using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface ITokenPriceService
    {
        Task<LatestPriceResponse?> GetLatestPriceUsdAsync(string tokenAddress);
    }
}
