using AssetManager.DTO;
using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetService
    {
        IEnumerable<ConsolidatedAssetDto> GetUserConsolidatedAssets(string userId);

        IEnumerable<AssetDto> GetUserAssets(string userId);


        bool CreateAsset(AssetDto assetDto, string userId);

    }
}
