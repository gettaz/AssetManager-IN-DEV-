namespace AssetManager.Interfaces
{
    public interface ICurrentPriceProvider
    {
        public Task<IEnumerable<string>> GetTickers();
    }
}