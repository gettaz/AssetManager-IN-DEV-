using AssetManager.DTO;
using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetService
    {
        IEnumerable<ConsolidatedAssetDto> GetUserAssets(string userId);

        bool CreateAsset(AssetDto assetDto);

    }
}
