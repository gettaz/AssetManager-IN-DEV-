namespace AssetManager.DTO
{
    public class TimelineSummaryDto
    {
        public string Name { get; set; }
        public IEnumerable<TimelineDataItem> Prices { get; set; }
    }

    public class TimelineDataItem
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}
