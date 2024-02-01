using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using AssetManager.DTO;
using AssetManager.DTO.AssetManager.DTO;
using Microsoft.AspNetCore.Authorization;
using AssetManager.ActionFilers.Filters;

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

        [HttpGet("assets")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetAssets()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var assets = _assetService.GetUserAssets(UserIdExtract());
                if (assets.IsNullOrEmpty())
                {
                    return NotFound(ModelState);
                }
                return Ok(assets);
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

        }

        [HttpGet("assets/category")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]

        public IActionResult GetAssetsByCategory(int categoryId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetAssetsByCategory(UserIdExtract(), categoryId);
            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("assets/broker")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AssetDto>))]

        public IActionResult GetAssetsByBroker(int brokerId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetAssetsByBroker(UserIdExtract(), brokerId);

            if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("assets/past")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Asset>))]

        public IActionResult GetPastHoldings()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetPastHoldings(UserIdExtract());

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

            if (!_assetService.CreateAsset(asset, UserIdExtract()))
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

        [HttpPost("assets/category")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult AddAssetCategory([FromBody] AssetCategoryDto assetCategoryDto)
        {

            if (assetCategoryDto == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var asset = _assetRepository.GetUserAssets(UserIdExtract()).FirstOrDefault(a => a.Id == assetCategoryDto.AssetId);
            var category = _categoryRepository.GetUserCategories(UserIdExtract()).FirstOrDefault(c => c.Id == assetCategoryDto.CategoryId);

            if (asset == null || category == null)
            {
                ModelState.AddModelError("", "Asset or Category not found");
                return NotFound(ModelState);
            }

            if (!_assetRepository.UpdateCategory(UserIdExtract(), assetCategoryDto.AssetId, assetCategoryDto.CategoryId))
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

            var asset = _assetRepository.GetUserAssets(UserIdExtract()).FirstOrDefault(a => a.Id == assetCategoryDto.AssetId);

            if (asset == null || asset.Category == null)
            {
                ModelState.AddModelError("", "Asset or Category not found");
                return NotFound(ModelState);
            }

            if (!_assetRepository.RemoveAssetCategory(UserIdExtract(), assetCategoryDto.AssetId))
            {
                ModelState.AddModelError("", "Something went wrong while adding asset to category");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed asset from category");
        }

        [HttpDelete("assets/{assetId}")]
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

        private string UserIdExtract()
        {
            var res1 = HttpContext.Items["UserId"];
            var res = HttpContext.Items["UserId"] as string;
            return res;
        }
    }
    }