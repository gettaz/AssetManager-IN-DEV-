namespace AssetManager.DTO
{
    public class ConsolidatedAssetDto
    {
        public string AssetName { get; set; }
        public string Ticker { get; set; }
        public double TotalAmount { get; set; }
        public double AveragePriceBought { get; set; }
        public string BrokerName { get; set; }
        public string Category { get; set; }
    }
}
