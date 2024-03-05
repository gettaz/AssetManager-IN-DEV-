using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using AssetManager.DTO;
using AssetManager.DTO.AssetManager.DTO;
using Microsoft.AspNetCore.Authorization;
using AssetManager.ActionFilers.Filters;
using Microsoft.EntityFrameworkCore;
using AssetManager.Enums;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class AssetsController : ControllerBase
    {
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetRepository _assetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrokerRepository _brokerRepository;
        private readonly IMapper _mapper;
        private readonly IAssetService _assetService;
        private readonly IPriceService _priceService;

        public AssetsController(ILogger<AssetsController> logger, IAssetRepository assetRepository, ICategoryRepository categoryRepository, IBrokerRepository brokerRepository, IMapper mapper, IAssetService assetService, IPriceService priceService)
        {
            _logger = logger;
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _brokerRepository = brokerRepository;
            _mapper = mapper;
            _assetService = assetService;
            _priceService = priceService;
        }

        [HttpGet("filter/{filterType}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetUserConsolidatedAssets(string filterType, string? brokerName, string? categoryName, string? ticker)
        {

            try
            {
                switch (Enum.Parse(typeof(AssetFilterType), filterType))
                {
                    case AssetFilterType.Summary:
                        {
                            var assets = _assetService.GetUserConsolidatedAssets(UserIdExtract());
                            return Ok(assets);
                        }
                    case AssetFilterType.Details:
                        {
                            var assets = _assetService.GetUserAssets(UserIdExtract(), categoryName, brokerName, ticker);
                            return Ok(assets);
                        }
                    case AssetFilterType.Past:
                        {
                            var assets = _assetRepository.GetPastHoldings(UserIdExtract());
                            return Ok(assets);
                        }
                    default:
                        {
                            return BadRequest(ModelState);
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in GetClassificationDistribution: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost()]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAsset([FromBody] AssetDto asset)
        {
            if (asset == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_assetService.CreateAsset(asset, UserIdExtract()))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut()]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateAsset([FromBody] AssetDto assetDto)
        {

            if (assetDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assetToChange = _assetRepository.GetUserAssets(UserIdExtract()).Where(ass => ass.Id == assetDto.Id).FirstOrDefault();

            if (assetToChange == null)
            {
                ModelState.AddModelError("", "Asset not found");
                return NotFound(ModelState);
            }

            if (assetToChange.UserId != UserIdExtract())
            {
                ModelState.AddModelError("", "User doesn't have the right to modify this asset");
                return Unauthorized(ModelState);
            }

            // Use reflection to map non-null properties from the DTO to the entity
            foreach (var prop in assetDto.GetType().GetProperties())
            {
                if (prop.GetValue(assetDto) != null)
                {
                    assetToChange.GetType().GetProperty(prop.Name)?.SetValue(assetToChange, prop.GetValue(assetDto));
                }
            }

            if (!_assetRepository.UpdateAsset(assetToChange))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{assetId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveAsset(int assetId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_assetRepository.DeleteAsset(UserIdExtract(), assetId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }

        [HttpGet("allowed")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        public async Task<IActionResult> GetAllowedTickers()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is invalid.");
                return BadRequest(ModelState);
            }

            var startTime = DateTime.UtcNow;
            _logger.LogInformation($"Starting to fetch allowed tickers at {startTime}.");

            var assets = await _priceService.GetAllowedTickers();

            var fetchTime = DateTime.UtcNow;
            _logger.LogInformation($"Finished fetching allowed tickers at {fetchTime}. Time taken: {fetchTime - startTime}.");

            if (assets == null || !assets.Any())
            {
                _logger.LogInformation("No assets found.");
                return NotFound();
            }

            var filterStartTime = DateTime.UtcNow;
            _logger.LogInformation($"Starting to filter assets at {filterStartTime}.");

            var filteredAssets = assets.ToList();

            var filterEndTime = DateTime.UtcNow;
            _logger.LogInformation($"Finished filtering assets at {filterEndTime}. Time taken: {filterEndTime - filterStartTime}.");

            return Ok(filteredAssets);
        }

        private string UserIdExtract()
        {
            var res = HttpContext.Items["UserId"] as string;
            return res;
        }
    }

}