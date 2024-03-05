namespace AssetManager.DTO
{
    public class TimelineSummaryDto
    {
        public string Name { get; set; }
        public IEnumerable<TimelineSummaryDto> Components { get; set; } = null;
        public IEnumerable<TimelineDataItem> Prices { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is TimelineSummaryDto))
            {
                return false;
            }
            return (Name == ((TimelineSummaryDto)obj).Name)
                && (Components == ((TimelineSummaryDto)obj).Components)
                 && (Prices == ((TimelineSummaryDto)obj).Prices);
        }
    }

    public class TimelineDataItem
    {
        public DateTime Date { get; set; }
        public double Price { get; set; } = 0;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is TimelineDataItem))
            {
                return false;
            }
            return (this.Price == ((TimelineDataItem)obj).Price)
                && (this.Date == ((TimelineDataItem)obj).Date);
        }
    }


}
