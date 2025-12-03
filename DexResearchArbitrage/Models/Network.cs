namespace DexResearchArbitrage.Models
{
    public enum Network
    {
        None = 0,
        Solana = 1,
        Ethereum = 2
    }
    
    public static class NetworkExtensions
    {
        public static string ToDisplayString(this Network network)
        {
            return network switch
            {
                Network.Solana => "Solana",
                Network.Ethereum => "Ethereum",
                _ => ""
            };
        }
    }
}
