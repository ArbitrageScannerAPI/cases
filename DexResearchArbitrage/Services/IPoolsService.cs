using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface IPoolsService
    {
        /// <summary>
        /// Gets all pools containing the specified token
        /// </summary>
        /// <param name="network">Target blockchain network</param>
        /// <param name="tokenAddress">Token contract address</param>
        /// <returns>List of pools with the token</returns>
        Task<List<PoolInfo>> GetPoolsByTokenAsync(Network network, string tokenAddress);
    }
}
