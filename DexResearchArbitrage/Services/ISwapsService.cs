using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface ISwapsService
    {
        // Added Network
        Task<PoolSwapsResponse?> GetPoolSwapsAsync(Network network, string poolAddress, int limit = 3000);
    }
}
