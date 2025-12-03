namespace DexResearchArbitrage.Models
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public string? TokenAddress { get; set; }
        public string? TokenSymbol { get; set; }
        public string? TokenName { get; set; }
        public string? ErrorMessage { get; set; }
        
        // TODO: Extend with additional token metadata from API response
    }
}
