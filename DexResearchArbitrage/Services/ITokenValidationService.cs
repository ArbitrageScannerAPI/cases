using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface ITokenValidationService
    {
        /// <summary>
        /// Validates if the given address is a valid token on the specified network
        /// </summary>
        /// <param name="network">Target blockchain network</param>
        /// <param name="tokenAddress">Token contract address</param>
        /// <returns>Validation result with token metadata if valid</returns>
        Task<TokenValidationResult> ValidateTokenAsync(Network network, string tokenAddress);
    }
}
