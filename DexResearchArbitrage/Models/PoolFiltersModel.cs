namespace DexResearchArbitrage.Models
{
    public class PoolFiltersModel
    {
        /// <summary>
        /// List of second token addresses to filter by (empty = no filter)
        /// </summary>
        public List<string> SecondTokenAddresses { get; set; } = new();

        /// <summary>
        /// Minimum TVL in USD (0 = no filter)
        /// </summary>
        public decimal MinTvlUsd { get; set; } = 0;
        
        /// <summary>
        /// Start date for swap count filter (null = no date filter)
        /// </summary>
        public DateTime? SwapsFromDate { get; set; }
        
        /// <summary>
        /// Minimum number of swaps (0 = no filter)
        /// </summary>
        public int MinSwapCount { get; set; } = 0;
        
        /// <summary>
        /// Minimum price difference percentage (0 = no filter)
        /// </summary>
        public decimal MinPriceDiffPercent { get; set; } = 0;
        
        /// <summary>
        /// Check if any filter is active
        /// </summary>
        public bool HasActiveFilters()
        {
            return SecondTokenAddresses.Count > 0 
                   || MinTvlUsd > 0 
                   || SwapsFromDate.HasValue 
                   || MinSwapCount > 0 
                   || MinPriceDiffPercent > 0;
        }
        
        /// <summary>
        /// Reset all filters to default values
        /// </summary>
        public void Reset()
        {
            SecondTokenAddresses.Clear();
            MinTvlUsd = 0;
            SwapsFromDate = null;
            MinSwapCount = 0;
            MinPriceDiffPercent = 0;
        }
    }
}
