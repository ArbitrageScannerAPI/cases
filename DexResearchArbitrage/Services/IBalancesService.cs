using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface IBalancesService
    {
        Task<AddressTokensBalanceResponse?> GetAddressTokensBalanceAsync(string poolAddress, int limit = 20);
    }
}
