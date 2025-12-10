using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface ISwapsService
    {
        Task<PoolSwapsResponse?> GetPoolSwapsAsync(string poolAddress, int limit = 10000);
    }
}
