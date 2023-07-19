using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using AssetManager.DTO;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public AssetsController(ILogger<AssetsController> logger, IAssetRepository assetRepository)
        {
            _logger = logger;
            _assetRepository = assetRepository;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Asset>))]
        [ProducesResponseType(404)]

        public IActionResult GetAssets(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assets = _assetRepository.GetUserAssets(userId);
          if (assets.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(assets);
        }

        [HttpGet("Assets/{userId}/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Asset>))]

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

        [HttpGet("{userId}/{broker}")]
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

        [HttpGet("{userId}/past")]
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

        [HttpPost("create")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAsset([FromBody] AssetDto asset)
        {
            if (asset == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assetMap = _mapper.Map<Asset>(asset);


            if (!_assetRepository.CreateAsset(assetMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateAsset([FromBody] AssetDto asset)
        {
            if (asset == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assetMap = _mapper.Map<Asset>(asset);


            if (!_assetRepository.UpdateAsset(assetMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("remove")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveAsset(int assetId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_assetRepository.DeleteAsset(assetId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
    }
    }