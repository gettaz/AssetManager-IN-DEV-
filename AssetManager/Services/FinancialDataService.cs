using AssetManager.Models;
using Newtonsoft.Json;
using System.Net.Http;
using AssetManager.Interfaces;

namespace AssetManager.Services
{
    public class FinancialDataService : IFinancialDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FinancialDataService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }

        public Task<IEnumerable<AssetSummary>> GetCurrentPrices(IEnumerable<Asset> assetTicker)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AssetSummary>> IFinancialDataService.GetPossibleAssets(string input)
        {
            throw new NotImplementedException();
        }
    }
}
