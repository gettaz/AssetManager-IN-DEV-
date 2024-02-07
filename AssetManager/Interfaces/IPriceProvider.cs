namespace AssetManager.Interfaces
{
    public interface IPriceProvider
    {
        Task<IEnumerable<string>> GetTickers();
    }
}
