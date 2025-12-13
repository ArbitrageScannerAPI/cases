using DexResearchArbitrage.Models;

namespace DexResearchArbitrage.Services
{
    public interface IBalancesService
    {
        // Добавили параметр Network network
        Task<AddressTokensBalanceResponse?> GetAddressTokensBalanceAsync(Network network, string poolAddress, int limit = 20);
    }
}

