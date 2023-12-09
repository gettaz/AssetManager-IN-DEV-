using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using AssetManager.DTO;
using AssetManager.DTO.AssetManager.DTO;
using AssetManager.Repository;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetRepository _assetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IAssetService _assetService;
        public AssetsController(ILogger<AssetsController> logger, IAssetRepository assetRepository, ICategoryRepository categoryRepository, IMapper mapper, IAssetService assetService)
        {
            _logger = logger;
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _assetService = assetService;
        }

        [HttpGet("{userId}/assets")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]
        [ProducesResponseType(404)]

        public IActionResult GetAssets(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetService.GetUserAssets(userId);
            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("{userId}/assets/category/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]

        public IActionResult GetAssetsByCategory(string userId, int categoryId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetAssetsByCategory(userId, categoryId);
            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("{userId}/assets/broker/{broker}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Asset>))]

        public IActionResult GetAssetsByBroker(string userId, string broker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetAssetsByBroker(userId, broker);

            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("{userId}/assets/past")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Asset>))]

        public IActionResult GetPastHoldings(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetPastHoldings(userId);

            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpPost("assets")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAsset([FromBody] AssetDto asset)
        {
            if (asset == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_assetService.CreateAsset(asset))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("assets/update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateAsset([FromBody] AssetDto assetDto)
        {
            if (assetDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assetToChange = _assetRepository.GetUserAssets(assetDto.UserId).Where(ass => ass.Id == assetDto.Id).FirstOrDefault();

            if (assetToChange == null)
            {
                ModelState.AddModelError("", "Asset not found");
                return NotFound(ModelState);
            }

            if (assetToChange.UserId != assetDto.UserId)
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

        [HttpPost("assets/category")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AddAssetCategory([FromBody] AssetCategoryDto assetCategoryDto)
        {
            if (assetCategoryDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asset = _assetRepository.GetUserAssets(assetCategoryDto.UserId).FirstOrDefault(a => a.Id == assetCategoryDto.AssetId);
            var category = _categoryRepository.GetUserCategories(assetCategoryDto.UserId).FirstOrDefault(c => c.Id == assetCategoryDto.CategoryId);

            if (asset == null || category == null)
            {
                ModelState.AddModelError("", "Asset or Category not found");
                return NotFound(ModelState);
            }

            if (!_assetRepository.UpdateCategory(assetCategoryDto.UserId, assetCategoryDto.AssetId, assetCategoryDto.CategoryId))
            {
                ModelState.AddModelError("", "Something went wrong while adding asset to category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully added asset to category");
        }

        [HttpDelete("assets/category")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveAssetCategory([FromBody] AssetCategoryDto assetCategoryDto)
        {
            if (assetCategoryDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asset = _assetRepository.GetUserAssets(assetCategoryDto.UserId).FirstOrDefault(a => a.Id == assetCategoryDto.AssetId);

            if (asset == null || asset.Category == null)
            {
                ModelState.AddModelError("", "Asset or Category not found");
                return NotFound(ModelState);
            }

            if (!_assetRepository.RemoveAssetCategory(assetCategoryDto.UserId, assetCategoryDto.AssetId))
            {
                ModelState.AddModelError("", "Something went wrong while adding asset to category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed asset from category");
        }

        [HttpDelete("{userId}/assets/{assetId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveAsset(string userId, int assetId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_assetRepository.DeleteAsset(userId, assetId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
    }
    }